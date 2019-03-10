using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mitto.IMessaging;

namespace Mitto.Routing.RabbitMQ.Publisher {
	internal class RequestManager {
		private ConcurrentDictionary<string, IRequest> Requests = new ConcurrentDictionary<string, IRequest>();

		public void Request<T>(IRequest pRequest) where T : IResponseMessage {
			if (Requests.TryAdd(pRequest.Message.ID, pRequest)) {
				pRequest.RequestTimedOut += RequestTimedOut;
				pRequest.Transmit();
			}
		}

		public void SetResponse(IResponseMessage pMessage) {
			if (
				Requests.ContainsKey(pMessage.ID) &&
				Requests.TryRemove(pMessage.ID, out IRequest objRequest)
			) {
				objRequest.RequestTimedOut -= RequestTimedOut;
				objRequest.SetResponse(pMessage);
			}
		}

		public MessageStatusType GetStatus(string pRequestID) {
			if (Requests.ContainsKey(pRequestID)) {
				return MessageStatusType.Queued;
			}
			return MessageStatusType.UnKnown;
		}

		private void RequestTimedOut(object sender, IRequest e) {
			SetResponse(MessagingFactory.Provider.GetResponseMessage(e.Message, new ResponseStatus(ResponseState.TimeOut)));
		}
	}
}
