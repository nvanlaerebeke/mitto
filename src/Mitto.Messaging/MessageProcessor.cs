using Mitto.IMessaging;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mitto.Messaging.Tests")]
namespace Mitto.Messaging {
	/// <summary>
	/// Provides handling of an incomming message
	/// </summary>
	public class MessageProcessor : IMessageProcessor {

		/// <summary>
		/// Internal class that does request management
		/// Example making a request and running its response
		/// </summary>
		internal IRequestManager RequestManager { get; set; } = new RequestManager();

		/// <summary>
		/// Takes in byte[] data, fetches the message it represents and passes
		/// that message to the RequestManager to processes said message
		/// 
		/// ToDo: instead of passing IQueue.IQueue something more elegant might be proper
		/// like the IClient already used in the RequestManager, that one can easily do 
		/// a IClient.Send(IMessage) or IClient.Request(objRequest)
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

			var objAction = MessagingFactory.Provider.GetAction(pClient, objMessage);
			if (objAction == null) { return; } // -- nothing to do

			switch (objMessage.Type) {
				case MessageType.Notification:
					try {
						((INotificationAction)objAction).Start();
					} catch (Exception) { /* ignore */ }
					break;
				case MessageType.Request:
					try {
						pClient.Transmit(
							MessagingFactory.Provider.GetByteArray(
								((IRequestAction)objAction).Start()
							)
						);
					} catch (Exception ex) {
						ResponseCode enmCode = (ex is MessagingException) ? ((MessagingException)ex).Code : ResponseCode.Error;
						var objResponse = MessagingFactory.Provider.GetResponseMessage(objMessage, enmCode);
						if (objResponse != null) {
							pClient.Transmit(MessagingFactory.Converter.GetByteArray(objResponse));
						}
					}
					break;
			}
		}

		public void Request<T>(IQueue.IQueue pClient, IMessage pMessage, Action<T> pCallback) where T : IResponseMessage {
			RequestManager.Request(pClient, pMessage, pCallback);
		}
	}
}