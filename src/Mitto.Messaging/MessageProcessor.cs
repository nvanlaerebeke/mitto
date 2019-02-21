using Mitto.IMessaging;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mitto.Messaging.Tests")]
namespace Mitto.Messaging {
	public class MessageProcessor : IMessageProcessor {
		internal IRequestManager RequestManager { get; set; } = new RequestManager();

		/// <summary>
		/// Takes in byte[] data, fetches the message it represents and passes
		/// that message to the RequestManager to processes said message
		/// </summary>
		/// <param name="pClient"></param>
		/// <param name="pData"></param>
		public void Process(IQueue.IQueue pClient, byte[] pData) {
			IMessage objMessage = MessagingFactory.Converter.GetMessage(pData);
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
						objAction.Start();
					} catch (Exception) { /* ignore */ }
					break;
				case MessageType.Request:
					try {
						pClient.Transmit(
							MessagingFactory.Converter.GetByteArray(
								objAction.Start()
							)
						);
					} catch (Exception ex) {
						ResponseCode enmCode = (ex is MessagingException) ? ((MessagingException)ex).Code : ResponseCode.Error;
						var objResponse = MessagingFactory.Converter.GetResponseMessage(objMessage, enmCode);
						if (objResponse != null) {
							pClient.Transmit(MessagingFactory.Converter.GetByteArray(objResponse));
						}
					}
					break;
			}
		}
	}
}