using IMessaging;
using System;

namespace Messaging.Base {
	internal class MessageRequest {
		private Delegate _objAction;

		internal void Request<R>(MessageClient pClient, RequestMessage pMessage, Action<R> action) where R : ResponseMessage {
			//only wait when the message actually expects a response, which is only RequestMessage and (Un)SubscribeMessage
			if (
				pMessage.Type == MessageType.Request ||
				pMessage.Type == MessageType.Subscribe ||
				pMessage.Type == MessageType.UnSubscribe ||
				pMessage.Type == MessageType.Event
			) {
				_objAction = action;
				pClient.Queue.Transmit(new IQueue.Message(pClient.ClientID, MessagingFactory.GetMessageCreator().GetBytes(pMessage)));
			} else if (pMessage.Type == MessageType.Notification) { // -- return the response right away for having successfully adding the msg to the queue
				pClient.Queue.Transmit(new IQueue.Message(pClient.ClientID, MessagingFactory.GetMessageCreator().GetBytes(pMessage)));
				SetResponse((R)MessageProvider.GetResponseMessage(pMessage, ResponseCode.Success));
			}
		}

		internal void SetResponse(ResponseMessage pResponse) {
			//((Action<ResponseMessage>)_objAction).Invoke(pResponse); // -- no idea how to get it like this as it's typesafe
			_objAction.DynamicInvoke(pResponse);
		}
	}
}