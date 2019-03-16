using System.Collections.Concurrent;
using Mitto.ILogging;
using Mitto.IMessaging;

namespace Mitto.Routing.RabbitMQ.Publisher {
	internal class RequestManager {
		private ILog Log {
			get {
				return LogFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
			}
		}


		private ConcurrentDictionary<string, Request> Requests = new ConcurrentDictionary<string, Request>();
		//private ConcurrentDictionary<string, SenderQueue> Queues = new ConcurrentDictionary<string, SenderQueue>();

		public void AddRequest(Request pRequest) {
			if (!Requests.TryAdd(pRequest.RequestID, pRequest)) {
				Log.Error("Unable to follow up request, request will time out");
			}
		}

		public void SetResponse(Frame pFrame) {
			if (Requests.ContainsKey(pFrame.MessageID)) {
				if (Requests.TryRemove(pFrame.MessageID, out Request objRequest)) {
					objRequest.SetResponse(pFrame);
				} else {
					//ToDo: Logging
				}
			} else {
				//ToDo: Logging
			}
		}
	}
}