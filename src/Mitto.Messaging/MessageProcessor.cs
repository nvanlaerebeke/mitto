using Mitto.IMessaging;
using System;

namespace Mitto.Messaging {
	public class MessageProcessor : IMessageProcessor {
		internal RequestManager RequestManager { get; set; } = new RequestManager();

		/// <summary>
		/// Takes in data and processes it
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

			try {
				var objActionType = MessagingFactory.Provider.GetActionType(objMessage);
				if (objActionType == null) { return; } // -- nothing to do

				switch (objMessage.Type) {
					case MessageType.Notification:
						objActionType.GetMethod("Start").Invoke(Activator.CreateInstance(objActionType, pClient, objMessage), new object[] { });
						break;
					case MessageType.Request:
					case MessageType.Subscribe:
					case MessageType.UnSubscribe:
					case MessageType.Event:
						pClient.Transmit(
							MessagingFactory.Converter.GetByteArray(
								(ResponseMessage)objActionType.GetMethod("Start").Invoke(
									Activator.CreateInstance(objActionType, pClient, objMessage), new object[] { }
								)
							)
						);
						break;
				}
			} catch (Exception ex) {
				if ( //other types than these don't expect a response
					objMessage.Type != MessageType.Response ||
					objMessage.Type != MessageType.Notification ||
					objMessage.Type != MessageType.Event
				) {
					ResponseCode enmCode = (ex is MessagingException) ? ((MessagingException)ex).Code : ResponseCode.Error;
					var objResponse = MessagingFactory.Converter.GetResponseMessage(objMessage, enmCode);
					if (objResponse != null) {
						pClient.Transmit(MessagingFactory.Converter.GetByteArray(objResponse));
					}
				}
			}
		}
	}
}