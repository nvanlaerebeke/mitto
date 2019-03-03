using Mitto.IMessaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mitto.Messaging.Tests")]
namespace Mitto.Messaging {
	/// <summary>
	/// Provides the Messages and Actions for the application
	/// 
	/// The messages are expected to have the following structure in the given namespace
	///     - Action
	///         - Notification
	///         - Request
	///     - Message
	///         - Notification
	///         - Request
	///         - Response
	///         
	/// Examples:
	///     <Namespace>.Request.Echo
	///     <Namespace>.Response.Echo
	///     <Namespace>.Action.Request.Echo
	///     
	/// 
	/// ToDo: Move messages under Mitto.Messaging.Message so that it's uniform with actions
	/// </summary>
	public class MessageProvider : IMessageProvider {

		/// <summary>
		/// The available IMessage classes
		/// </summary>
		internal Dictionary<MessageType, Dictionary<string, Type>> Types { get; } = new Dictionary<MessageType, Dictionary<string, Type>>();

		/// <summary>
		/// The available IAction classes
		/// </summary>
		internal Dictionary<MessageType, Dictionary<string, Type>> Actions { get; } = new Dictionary<MessageType, Dictionary<string, Type>>();

		/// <summary>
		/// The available SubscriptionHandler classes
		/// </summary>
		internal Dictionary<string, object> SubscriptionHandlers { get; } = new Dictionary<string, object>();

		/// <summary>
		/// Loads the types present in the specified Namespaces
		/// The build-in types will be automatically added first
		/// 
		/// It is possible to overwrite the build-in message types and actions by your 
		/// own implementation when creating the class in your own namespaces
		/// 
		/// Note that the order of the passed namespaces will be respected and the last
		/// in the list will win.
		/// </summary>
		/// <param name="pNamespace"></param>
		public MessageProvider(IEnumerable<string> pNamespaces) {
			LoadTypes(typeof(MessageProvider).Namespace);
			foreach (var strNamespace in pNamespaces) {
				if (strNamespace != typeof(MessageProvider).Namespace) {
					LoadTypes(strNamespace);
				}
			}
		}

		/// <summary>
		/// Loads the types present in the specified Namespace
		/// The build-in types will be automatically added first
		/// 
		/// It is possible to overwrite the build-in message types and actions by your 
		/// own implementation when creating the class in your own namespace
		/// </summary>
		/// <param name="pNamespace"></param>
		public MessageProvider(string pNamespace) {
			if (pNamespace != typeof(MessageProvider).Namespace) {
				LoadTypes(typeof(MessageProvider).Namespace);
			}
			LoadTypes(pNamespace);
		}

		/// <summary>
		/// Loads the base types only
		/// </summary>
		public MessageProvider() {
			LoadTypes(typeof(MessageProvider).Namespace);
		}

		/// <summary>
		/// Loads the types based on the provided namespace
		/// </summary>
		/// <param name="pNamespace"></param>
		private void LoadTypes(string pNamespace) {
			Dictionary<MessageType, string> dicMessageNamespaces = new Dictionary<MessageType, string> {
				{ MessageType.Notification, ".Notification" },
				{ MessageType.Request, ".Request" },
				{ MessageType.Response, ".Response" },
				{ MessageType.Sub, ".Subscribe" },
				{ MessageType.UnSub, ".UnSubscribe" }
			};

			//Messages
			foreach (var objKvp in dicMessageNamespaces) {
				foreach (var objType in GetByNamespace(pNamespace + objKvp.Value)) {
					AddMessageType(objKvp.Key, objType);
				}
			}

			//Actions
			Dictionary<MessageType, string> dicActionNamespaces = new Dictionary<MessageType, string> {
				{ MessageType.Notification, ".Action.Notification" },
				{ MessageType.Request, ".Action.Request" },
				{ MessageType.Sub, ".Action.Subscribe" },
				{ MessageType.UnSub, ".Action.UnSubscribe" }
			};

			foreach (var objKvp in dicActionNamespaces) {
				foreach (var objType in GetByNamespace(pNamespace + objKvp.Value)) {
					AddActionType(objKvp.Key, objType);
				}
			}

			//Subscription handlers
			foreach (var objType in GetByNamespace(pNamespace + ".Action.SubscriptionHandler")) {
				AddSubscriptionHandlerType(objType.Name, objType);
			}
		}

		/// <summary>
		/// Adds the provided IMessage type to the cached types
		/// 
		/// Messages must implement the IMessage interface and may not be abstract classes
		///
		/// When passing a message with a name that already exists it will overwrite 
		/// the already existing cache version.
		/// 
		/// MessageType/Name combination should be unique
		/// </summary>
		/// <param name="pMessageType"></param>
		/// <param name="pType"></param>
		private void AddMessageType(MessageType pMessageType, Type pType) {
			if (pType.IsAbstract || !pType.GetInterfaces().Contains(typeof(IMessage))) { return; }

			if (!Types.ContainsKey(pMessageType)) {
				Types.Add(pMessageType, new Dictionary<string, Type> { { pType.Name, pType } });
			} else {
				if (!Types[pMessageType].ContainsKey(pType.Name)) {
					Types[pMessageType].Add(pType.Name, pType);
				} else {
					Types[pMessageType][pType.Name] = pType;
				}
			}
		}

		/// <summary>
		/// Adds the provided IAction type to the cached types
		/// 
		/// Actions must implement the IRequestAction or INotification interfaces
		/// 
		/// When passing an action with a name that already exists it will overwrite 
		/// the already existing cache version.
		/// 
		/// MessageType/Name combination should be unique
		/// </summary>
		/// <param name="pMessageType"></param>
		/// <param name="pType"></param>
		private void AddActionType(MessageType pMessageType, Type pType) {
			if (pType.IsAbstract || !pType.GetInterfaces().Contains(typeof(IAction))) { return; }

			if (!Actions.ContainsKey(pMessageType)) {
				Actions.Add(pMessageType, new Dictionary<string, Type> { { pType.Name, pType } });
			} else {
				if (!Actions[pMessageType].ContainsKey(pType.Name)) {
					Actions[pMessageType].Add(pType.Name, pType);
				} else {
					Actions[pMessageType][pType.Name] = pType;
				}
			}
		}

		/// <summary>
		/// Adds a subscription handler of type pType to the provider list
		/// Overwrites any existing type present for that name
		/// 
		/// ToDo: add an interface like IAction 
		/// </summary>
		/// <param name="pName"></param>
		/// <param name="pType"></param>
		private void AddSubscriptionHandlerType(string pName, Type pType) {
			if (pType.IsAbstract) { return; }

			var arrKeys = new List<string>() { pName };
			foreach (var objInterface in pType.GetInterfaces()) {
				if(objInterface.Namespace.Equals(pType.Namespace)) {
					arrKeys.Add(objInterface.Name);
				}
			}

			var obj = Activator.CreateInstance(pType);
			foreach (var strKey in arrKeys) {
				if (!SubscriptionHandlers.ContainsKey(strKey)) {
					SubscriptionHandlers.Add(strKey, obj);
				} else {
					SubscriptionHandlers[strKey] = obj;
				}
			}
		}

		/// <summary>
		/// Gets all Request/Response/Notification messages and the Actions from the provided namespace
		/// </summary>
		/// <param name="pNamespace"></param>
		/// <returns>List<Type></returns>
		private List<Type> GetByNamespace(string pNamespace) {
			var lstTypes = new List<Type>();

			//Loop over all loaded assemblies so no classes are missed
			foreach (
				var ass in AppDomain.CurrentDomain.GetAssemblies()
			) {
				try {
					//foreach loaded assembly, get all it's types that match the given namespace
					var arrTypes = (from t in ass.GetTypes() where t.IsClass && t.Namespace == pNamespace select t);
					foreach (
						var objType in arrTypes
					) {
						if (
							objType.GetInterfaces().Contains(typeof(IRequestMessage)) ||
							objType.GetInterfaces().Contains(typeof(IResponseMessage)) ||
							objType.GetInterfaces().Contains(typeof(IAction)) ||
							objType.Namespace.Contains(".Action.SubscriptionHandler") //is a generic type, easy solution is to just check the namespace string instead of  IsSubclassOf(typeof(Action.BaseAction<T>))
							//objType.IsSubclassOf(typeof(NotificationMessage)) ||
						) {
							lstTypes.Add(objType);
						}
					}
				} catch (Exception) { }
			}
			return lstTypes;
		}

		/// <summary>
		/// Gets an IMessage based on it's data (byte array)
		/// See the Frame object for more information on what the bytes are represent
		/// </summary>
		/// <param name="pData"></param>
		/// <returns>IMessage</returns>
		public IMessage GetMessage(byte[] pData) {
			var objFrame = new Frame(pData);
			if (
				Types.ContainsKey(objFrame.Type) &&
				Types[objFrame.Type].ContainsKey(objFrame.Name)
			) {
				return MessagingFactory.Converter.GetMessage(Types[objFrame.Type][objFrame.Name], objFrame.Data);
			}
			return null;
		}

		/// <summary>
		/// Returns the response for a specific request message with the provided response code
		/// 
		/// When no specific response class can be found, the response will be a Response.ACK
		/// 
		/// 
		/// ToDo: will have to convert the enum to an actual int so that the application can pass it's errors and 
		/// so that there can be more than 255 return codes
		/// </summary>
		/// <param name="pMessage"></param>
		/// <param name="pCode"></param>
		/// <returns></returns>
		public IResponseMessage GetResponseMessage(IRequestMessage pMessage, ResponseCode pCode) {
			Type objResponseType = typeof(Response.ACK); // -- default
			if (
				Types.ContainsKey(MessageType.Response) &&
				Types[MessageType.Response].ContainsKey(pMessage.Name)
			) {
				objResponseType = Types[MessageType.Response][pMessage.Name];
			}
			return ((IResponseMessage)Activator.CreateInstance(objResponseType, pMessage, pCode));
		}

		/// <summary>
		/// Gets the IAction for a specific IMessage (Request/Notification)
		/// </summary>
		/// <param name="pClient"></param>
		/// <param name="pMessage"></param>
		/// <returns></returns>
		public IAction GetAction(IClient pClient, IRequestMessage pMessage) {
			if (
				Actions.ContainsKey(pMessage.Type) &&
				Actions[pMessage.Type].ContainsKey(pMessage.Name)
			) {
				var type = (Actions[pMessage.Type][pMessage.Name]);
				var obj = Activator.CreateInstance(Actions[pMessage.Type][pMessage.Name], pClient, pMessage);
				return (IAction)obj;
			}
			return null;
		}

		public T GetSubscriptionHandler<T>() { // where T : ISubscriptionHandler<IRequestMessage, IRequestMessage> {
			return (SubscriptionHandlers.ContainsKey(typeof(T).Name)) ? (T)SubscriptionHandlers[typeof(T).Name] : default(T);
		}

		/// <summary>
		/// Gets the byte[] representation for the given IMessage
		/// 
		/// Note: uses the configured IMessageConverter
		/// See frame documentation on how a frame should look
		/// </summary>
		/// <param name="pMessage"></param>
		/// <returns></returns>
		public byte[] GetByteArray(IMessage pMessage) {
			return new Frame(
				pMessage.Type,
				pMessage.Name,
				MessagingFactory.Converter.GetByteArray(pMessage)
			).GetByteArray();
		}
	}
}