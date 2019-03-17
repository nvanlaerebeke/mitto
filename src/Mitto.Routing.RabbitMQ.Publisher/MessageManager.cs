using System.Collections.Concurrent;
using Mitto.ILogging;
using Mitto.IMessaging;
using Mitto.IRouting;

namespace Mitto.Routing.RabbitMQ.Publisher {
	internal class MessageManager {
		private ILog Log {
			get {
				return LogFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
			}
		}
		private readonly SenderQueue MainQueue;
		private readonly ConcurrentDictionary<string, Request> Requests;

		public MessageManager(SenderQueue pMainQueue) {
			MainQueue = pMainQueue;
			Requests = new ConcurrentDictionary<string, Request>();
		}

		public void AddRequest(Request pRequest) {
			if (!Requests.TryAdd(pRequest.RequestID, pRequest)) {
				Log.Error("Unable to follow up request, request will time out");
			}
		}

		public void Process(RoutingFrame pFrame) {
			if (pFrame.FrameType == RoutingFrameType.Control) {

			} else {
				var objMessageFrame = new Frame(pFrame.Data);
				if (objMessageFrame.Type == MessageType.Response) {
					SetResponse(objMessageFrame.ID, pFrame);
				} else if (objMessageFrame.Type == MessageType.Sub) {
					//ToDo: broadcast to all consumers
					MainQueue.Transmit(new RabbitMQFrame(RabbitMQFrameType.Messaging, RouterProvider.ID, pFrame.GetBytes()));
//				} else if (objMessageFrame.Type == MessageType.Notification) {
				} else {
					MainQueue.Transmit(new RabbitMQFrame(RabbitMQFrameType.Messaging, RouterProvider.ID, pFrame.GetBytes()));
				}
			}
		}

		private void SetResponse(string pRequestID, RoutingFrame pFrame) {
			if (Requests.ContainsKey(pRequestID)) {
				if (Requests.TryRemove(pRequestID, out Request objRequest)) {
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