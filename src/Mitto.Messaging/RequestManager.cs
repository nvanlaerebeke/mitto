using Mitto.IMessaging;
using System;
using System.Collections.Concurrent;

namespace Mitto.Messaging {
	public class RequestManager {
		private ConcurrentDictionary<string, MessageRequest> _dicRequests = new ConcurrentDictionary<string, MessageRequest>();

		public void Request<R>(MessageClient pClient, IMessage pMessage, Action<R> pAction) where R : IResponseMessage {
			lock (_dicRequests) {
				var obj = new MessageRequest();
				obj.Request<R>(pClient, pMessage, pAction);
				_dicRequests.TryAdd(pMessage.ID, obj);
			}
		}

		internal void SetResponse(IResponseMessage pMessage) {
			lock (_dicRequests) {
				if (_dicRequests.ContainsKey(pMessage.ID)) {
					_dicRequests[pMessage.ID].SetResponse(pMessage);
				}
			}
		}
	}
}