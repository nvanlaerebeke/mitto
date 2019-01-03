using IMessaging;
using System;
using System.Collections.Concurrent;

namespace Messaging.Base {
	public static class Requester {
		private static ConcurrentDictionary<string, Action<ResponseMessage>> _dicRequests = new ConcurrentDictionary<string, Action<ResponseMessage>>();

		public static void Send<R>(MessageClient pClient, RequestMessage pMessage, Action<R> pAction) where R : ResponseMessage {
			//only wait when the message actually expects a response, which is only RequestMessage and (Un)SubscribeMessage
			if (
				pMessage.Type == MessageType.Request ||
				pMessage.Type == MessageType.Subscribe ||
				pMessage.Type == MessageType.UnSubscribe ||
				pMessage.Type == MessageType.Event
			) {
				lock (_dicRequests) {
					_dicRequests.TryAdd(pMessage.ID, o => pAction((R)o));
				}
				pClient.Queue.Transmit(new IQueue.Message(pClient.ClientID, MessagingFactory.GetMessageCreator().GetBytes(pMessage)));
			} else if (pMessage.Type == MessageType.Notification) { // -- return the response right away for having successfully adding the msg to the queue
				pClient.Queue.Transmit(new IQueue.Message(pClient.ClientID, MessagingFactory.GetMessageCreator().GetBytes(pMessage)));
				pAction((R)MessageProvider.GetResponseMessage(pMessage, ResponseCode.Success));
			}
		}

		public static void SetResponse(ResponseMessage pMessage) {
			lock (_dicRequests) {
				if (_dicRequests.ContainsKey(pMessage.ID)) {
					_dicRequests[pMessage.ID](pMessage);
				}
			}
		}
	}
}