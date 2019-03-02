using Mitto.IMessaging;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mitto.Messaging.Tests")]
namespace Mitto.Messaging {
	/// <summary>
	/// Provides handling of an incomming message and making requests
	/// </summary>
	public class MessageProcessor : IMessageProcessor {

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
		/// </summary>
		/// <param name="pClient"></param>
		/// <param name="pData"></param>
		public void Process(IQueue.IQueue pClient, byte[] pData) {
			IMessage objMessage = MessagingFactory.Provider.GetMessage(pData);

			// --- don't know what we're receiving, so skip it
			if (objMessage == null) { return; }

			//Message is a response on an action, so no action needs to run
			//set the response and return
			if (objMessage.Type == MessageType.Response) {
				RequestManager.SetResponse(objMessage as IResponseMessage);
				return;
			}

			var objClient = new Client(pClient, RequestManager);
			ActionManager.RunAction(
				objClient, 
				objMessage, 
				MessagingFactory.Provider.GetAction(objClient, objMessage)
			);
		}

		/// <summary>
		/// Starts a Request by passing it to the RequestManager
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="pClient"></param>
		/// <param name="pMessage"></param>
		/// <param name="pCallback"></param>
		public void Request<T>(IQueue.IQueue pClient, IMessage pMessage, Action<T> pAction) where T : IResponseMessage {
			RequestManager.Request<T>(new Request<T>(new Client(pClient, RequestManager), pMessage, pAction));
		}

		/// <summary>
		/// Gets the current status of a message
		/// First checks if the message is being processed, if it is not, 
		/// check if it's still in the queue
		/// </summary>
		/// <param name="pRequestID"></param>
		/// <returns></returns>
		public MessageStatusType GetStatus(string pRequestID) {
			var objActionStatus = ActionManager.GetStatus(pRequestID);
			if(objActionStatus == MessageStatusType.UnKnown) {
				return RequestManager.GetStatus(pRequestID);
			}
			return objActionStatus;
		}
	}
}