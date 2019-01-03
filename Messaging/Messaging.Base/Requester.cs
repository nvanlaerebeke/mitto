using System;
using System.Collections.Concurrent;

namespace Messaging.Base {
	public static class Requester {
		private static ConcurrentDictionary<string, MessageRequest> _dicRequests = new ConcurrentDictionary<string, MessageRequest>();

		public static void Request<R>(MessageClient pClient, RequestMessage pMessage, Action<R> pAction) where R : ResponseMessage {
			lock (_dicRequests) {
				var obj = new MessageRequest();
				obj.Request<R>(pClient, pMessage, pAction);
				_dicRequests.TryAdd(pMessage.ID, obj);
			}
		}

		internal static void SetResponse(ResponseMessage pMessage) {
			lock (_dicRequests) {
				if (_dicRequests.ContainsKey(pMessage.ID)) {
					_dicRequests[pMessage.ID].SetResponse(pMessage);
				}
			}
		}
	}
}