using Mitto.ILogging;
using Mitto.IMessaging;
using Mitto.IRouting;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mitto.Messaging.Tests")]

namespace Mitto.Messaging {

    /// <summary>
    /// Provides handling of an incoming message and making requests
    /// </summary>
    public class MessageProcessor : IMessageProcessor {

        private ILog Log {
            get { return LogFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType); }
        }

        /// <summary>
        /// Create a MessageProcessor with a custom IRequestManager and IActionManager
        /// </summary>
        /// <param name="pRequestManager"></param>
        /// <param name="pActionManager"></param>
        internal MessageProcessor(IRequestManager pRequestManager, IActionManager pActionManager) {
            RequestManager = pRequestManager;
            ActionManager = pActionManager;
        }

        /// <summary>
        /// Creates the MessageProcessor
        /// </summary>
        public MessageProcessor() {
            RequestManager = new RequestManager();
            ActionManager = new ActionManager();
        }

        /// <summary>
        /// Internal class that does request management
        /// Example making a request and running its response
        /// </summary>
        private IRequestManager RequestManager { get; set; }

        /// <summary>
        /// Internal class that does the action management
        /// Example starting the action and its error handling
        /// </summary>
        private IActionManager ActionManager { get; set; }

        /// <summary>
        /// Takes in byte[] data, fetches the message it represents
        /// and passes it to the RequestManager in case it's an IResponseMessage,
        /// otherwise an IAction is created and is passed to the IActionManager
        /// to handle the IAction lifetime
        ///
        /// ToDo: Pass IRequest instead of the IRouter + byte[] that should actually be a RoutingFrame)
        /// </summary>
        /// <param name="pClient"></param>
        /// <param name="pData"></param>
        public void Process(IRouter pClient, byte[] pData) {
            IMessage objMessage = MessagingFactory.Provider.GetMessage(pData);

            // --- don't know what we're receiving, so skip it
            if (objMessage == null) {
                Log.Error($"Received unknown/corrupt message");
                return;
            }

            //Message is a response on an action, so no action needs to run
            //set the response and return
            if (objMessage.Type == MessageType.Response) {
                RequestManager.SetResponse(objMessage as IResponseMessage);
                return;
            }

            var objClient = new Client(pClient, RequestManager);
            ActionManager.RunAction(
                objClient,
                objMessage as IRequestMessage,
                MessagingFactory.Provider.GetAction(objClient, objMessage as IRequestMessage)
            );
        }

        /// <summary>
        /// Starts a Request by passing it to the RequestManager
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pClient"></param>
        /// <param name="pMessage"></param>
        /// <param name="pCallback"></param>
        public void Request<T>(IRouter pClient, IRequestMessage pMessage, Action<T> pAction) where T : IResponseMessage {
            RequestManager.Request<T>(new Request<T>(new Client(pClient, RequestManager), pMessage, pAction));
        }

        /// <summary>
        /// Gets the current status of a message
        /// First checks if the message is being processed, if it is not,
        /// check if it's still in the queue
        /// </summary>
        /// <param name="pRequestID"></param>
        /// <returns></returns>
        public MessageStatus GetStatus(string pRequestID) {
            var objActionStatus = ActionManager.GetStatus(pRequestID);
            if (objActionStatus == MessageStatus.UnKnown) {
                return RequestManager.GetStatus(pRequestID);
            }
            return objActionStatus;
        }
    }
}