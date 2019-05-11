using Mitto.IMessaging;
using Mitto.IRouting;
using Mitto.Logging;
using Mitto.Messaging.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mitto.Messaging.Tests")]

namespace Mitto.Messaging {

    /// <summary>
    /// ToDo: merge requests and responses into one dictionary, naming convention prevents collisions in name usage
    /// </summary>
    public class MessageProvider : IMessageProvider {

        private ILog Log {
            get { return LoggingFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType); }
        }

        /// <summary>
        /// The available IRequestMessage classes
        /// </summary>
        internal Dictionary<string, Type> Requests { get; } = new Dictionary<string, Type>();

        /// <summary>
        /// The available IResponseMessage classes
        /// </summary>
        internal Dictionary<string, Type> Responses = new Dictionary<string, Type>();

        /// <summary>
        /// The available IAction classes
        /// </summary>
        internal List<ActionInfo> Actions { get; } = new List<ActionInfo>();

        /// <summary>
        /// The available SubscriptionHandler classes
        /// </summary>
        internal List<SubscriptionInfo> SubscriptionHandlers { get; } = new List<SubscriptionInfo>();

        /// <summary>
        /// Loads the types present in any of the referenced assemblies
        /// The build-in types will be automatically added first
        ///
        /// It is possible to overwrite the build-in message types and actions by your
        /// own implementation when creating the class in your own namespaces
        ///
        /// Note that when implementing the same message multiple times the last one will win
        /// </summary>
        public MessageProvider() { }

        public void Load(IEnumerable<AssemblyName> pAssemblies) {
            List<Type> lstSupportedTypes = new List<Type>() {
                typeof(INotificationMessage),
                typeof(IRequestMessage),
                typeof(IResponseMessage),
                typeof(IAction),
                typeof(ISubscriptionHandler)
            };

            Log.Info("Loading all referenced assemblies");
            var arrAssemblies = new List<Assembly>();
            foreach (var strAss in pAssemblies) {
                try {
                    arrAssemblies.Add(Assembly.Load(strAss));
                } catch {
                    Log.Error($"Unable to load {strAss}");
                }
            }

            GetAssemblies(ref arrAssemblies, Assembly.GetEntryAssembly());
            arrAssemblies.OrderBy(a => a.FullName.Contains("Mitto.Messaging") ? 1 : a.FullName.Contains("Mitto.") ? 2 : 3).ToList();

            Log.Info($"Found {arrAssemblies.Count} Assemblies");

            foreach (var ass in arrAssemblies) {
                try {
                    Log.Info($"Loading types from {ass.FullName}");
                    var types = (
                        from t in ass.GetTypes()
                        where
                            t.IsClass &&
                            !t.IsAbstract &&
                            t.GetInterfaces().Any(i => lstSupportedTypes.Contains(i))
                        select t
                    ).ToList();
                    (
                        from t in ass.GetTypes()
                        where
                            t.IsClass &&
                            !t.IsAbstract &&
                            t.GetInterfaces().Any(i => lstSupportedTypes.Contains(i))
                        select t
                    ).ToList().ForEach(t =>
                    {
                        var lstInterfaces = t.GetInterfaces();
                        if (lstInterfaces.Contains(typeof(IAction))) {
                            var tmpType = lstInterfaces.Where(i => i.IsInterface && i.IsAbstract && i.FullName.Contains("IRequestAction") || i.FullName.Contains("INotificationAction")).FirstOrDefault();
                            if (tmpType != null) {
                                Type objRequestType = (tmpType.GenericTypeArguments.Length > 0) ? tmpType.GenericTypeArguments[0] : null;
                                Type objResponseType = (tmpType.GenericTypeArguments.Length > 1) ? tmpType.GenericTypeArguments[1] : null;
                                if (objRequestType != null) {
                                    var objActionType = new ActionInfo(objRequestType, objResponseType, t);
                                    Actions.RemoveAll(a => a.ActionType.Name.Equals(objActionType.ActionType.Name));
                                    Actions.Add(objActionType);
                                    if (objActionType.ResponseType != null) {
                                        Log.Info($"Found action {objActionType.ActionType.Name} with request {objActionType.RequestType.Name} and response {objActionType.ResponseType.Name}");
                                    } else {
                                        Log.Info($"Found action {objActionType.ActionType.Name} with request {objActionType.RequestType.Name}");
                                    }
                                }
                            }
                        } else if (
                            lstInterfaces.Contains(typeof(IRequestMessage)) ||
                            lstInterfaces.Contains(typeof(INotificationMessage))
                        ) {
                            Requests.Remove(t.Name);
                            Requests.Add(t.Name, t);
                            Log.Info($"Found Request of type {t.Name}");
                        } else if (lstInterfaces.Contains(typeof(IResponseMessage))) {
                            Responses.Remove(t.Name);
                            Responses.Add(t.Name, t);
                            Log.Info($"Found Response of type {t.Name}");
                        } else if (lstInterfaces.Contains(typeof(ISubscriptionHandler))) {
                            var tmpType = t.GetInterfaces().Where(i => i.IsInterface && i.FullName.Contains("ISubscriptionHandler") && i.IsGenericType).FirstOrDefault();
                            if (tmpType != null) {
                                Type objSubType = (tmpType.GenericTypeArguments.Length > 0) ? tmpType.GenericTypeArguments[0] : null;
                                Type objUnSubType = (tmpType.GenericTypeArguments.Length > 1) ? tmpType.GenericTypeArguments[1] : null;
                                Type objNotifyType = (tmpType.GenericTypeArguments.Length > 2) ? tmpType.GenericTypeArguments[2] : null;
                                if (objSubType != null && objUnSubType != null && objNotifyType != null) {
                                    var objHandler = (ISubscriptionHandler)Activator.CreateInstance(t);
                                    var objSubscriptionInfo = new SubscriptionInfo(objSubType, objUnSubType, objNotifyType, objHandler);
                                    SubscriptionHandlers.RemoveAll(s => s.HandlerType.Equals(t.Name));
                                    SubscriptionHandlers.Add(objSubscriptionInfo);
                                    Log.Info($"Found subscription handler {objSubscriptionInfo.HandlerType.Name}");
                                }
                            }
                        }
                    });
                } catch (Exception ex) {
                    Log.Error("Unable to load types");
                    Log.Error(ex.Message);
                }
            }
        }

        private void GetAssemblies(ref List<Assembly> pLoaded, Assembly pRoot) {
            var arrReferenced = pRoot.GetReferencedAssemblies();
            foreach (var objName in arrReferenced) {
                var ass = Assembly.Load(objName);
                if (!pLoaded.Contains(ass)) {
                    pLoaded.Add(ass);
                    GetAssemblies(ref pLoaded, ass);
                }
            }
        }

        /// <summary>
        /// Gets an IMessage based on it's data (byte array)
        /// See the Frame object for more information on what the bytes are represent
        /// </summary>
        /// <param name="pData"></param>
        /// <returns>IMessage</returns>
        public IMessage GetMessage(byte[] pData) {
            Frame objFrame = new Frame(pData);
            Type objType = null;
            try {
                if (objFrame.Type != MessageType.Response) {
                    if (Requests.ContainsKey(objFrame.Name)) {
                        objType = Requests[objFrame.Name];
                    }
                } else {
                    if (Responses.ContainsKey(objFrame.Name)) {
                        objType = Responses[objFrame.Name];
                    }
                }
            } catch {
                Log.Error("Invalid data received, ignoring...");
                return null;
            }
            if (objType != null) {
                return MessagingFactory.Converter.GetMessage(objType, objFrame.Data);
            }
            return null;
        }

        /// <summary>
        /// Returns the response for a specific request message with the provided response code
        ///
        /// When no specific response class can be found, the response will be a Response.ACK
        ///
        ///
        /// ToDo: will have to convert the enumeration to an actual int so that the application can pass it's errors and
        /// so that there can be more than 255 return codes
        /// </summary>
        /// <param name="pMessage"></param>
        /// <param name="pCode"></param>
        /// <returns></returns>
        public IResponseMessage GetResponseMessage(IRequestMessage pMessage, ResponseStatus pStatus) {
            Type objResponseType = typeof(ACKResponse); // -- default

            var objInfo = Actions.Where(a => a.RequestType.Name == pMessage.Name).FirstOrDefault();
            if (objInfo != null) {
                objResponseType = objInfo.ResponseType;
            }

            return Activator.CreateInstance(objResponseType, pMessage, pStatus) as IResponseMessage;
        }

        public IResponseMessage GetResponseMessage(Type pType, IRequestMessage pMessage, ResponseStatus pStatus) {
            return Activator.CreateInstance(pType, pMessage, pStatus) as IResponseMessage;
        }

        /// <summary>
        /// Gets the IAction for a specific IMessage (Request/Notification)
        /// </summary>
        /// <param name="pClient"></param>
        /// <param name="pMessage"></param>
        /// <returns></returns>
        public IAction GetAction(IClient pClient, IRequestMessage pMessage) {
            ActionInfo objInfo = Actions.Where(a => a.RequestType.Name == pMessage.Name).FirstOrDefault();
            return (objInfo != null) ? (IAction)Activator.CreateInstance(objInfo.ActionType, pClient, pMessage) : null;
        }

        public T GetSubscriptionHandler<T>() {
            var objInfo = SubscriptionHandlers.FirstOrDefault(s => s.HandlerType.Equals(typeof(T)));
            ISubscriptionHandler objHandler = null;
            if (objInfo != null) {
                objHandler = objInfo.Handler;
            }
            return (T)objHandler;
        }

        public ISubscriptionHandler GetSubscriptionHandler(IMessage pMessage) {
            var objInfo = SubscriptionHandlers.FirstOrDefault(s =>
                s.NotifyType.Name.Equals(pMessage.GetType().Name) ||
                s.SubType.Name.Equals(pMessage.GetType().Name) ||
                s.UnSubType.Name.Equals(pMessage.GetType().Name)
);
            if (objInfo != null) {
                return objInfo.Handler;
            }
            return null;
        }

        internal class ActionInfo : IEquatable<ActionInfo> {
            public Type RequestType { get; private set; }
            public Type ResponseType { get; private set; }
            public Type ActionType { get; private set; }

            public ActionInfo(Type pRequestType, Type pResponseType, Type pActionType) {
                RequestType = pRequestType;
                ResponseType = pResponseType;
                ActionType = pActionType;
            }

            public bool Equals(ActionInfo pActionInfo) {
                return (
                    this == pActionInfo ||
                    (
                        this.RequestType.FullName.Equals(pActionInfo.RequestType.FullName) &&
                        this.ResponseType.FullName.Equals(pActionInfo.ResponseType.FullName) &&
                        this.ActionType.FullName.Equals(pActionInfo.ActionType.FullName)
                    )
                );
            }
        }

        internal class SubscriptionInfo : IEquatable<SubscriptionInfo> {
            public readonly Type SubType;
            public readonly Type UnSubType;
            public readonly Type NotifyType;
            public readonly Type HandlerType;
            public readonly ISubscriptionHandler Handler;

            public SubscriptionInfo(Type pSubType, Type pUnSubType, Type pNotifyType, ISubscriptionHandler pHandler) {
                SubType = pSubType;
                UnSubType = pUnSubType;
                NotifyType = pNotifyType;
                HandlerType = pHandler.GetType();
                Handler = pHandler;
            }

            public bool Equals(SubscriptionInfo pInfo) {
                return (
                    this == pInfo ||
                    (
                        this.SubType == pInfo.SubType &&
                        this.UnSubType == pInfo.UnSubType &&
                        this.NotifyType == pInfo.NotifyType
                    )
                );
            }
        }
    }
}