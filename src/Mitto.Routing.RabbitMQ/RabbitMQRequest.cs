using Mitto.ILogging;
using Mitto.IRouting;
using Mitto.Routing.Request;
using Mitto.Routing.Response;
using Mitto.Utilities;
using System;
using System.Threading.Tasks;

namespace Mitto.Routing.RabbitMQ {
	public class RabbitMQRequest : IRequest {
		private static ILog Log = LogFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public string ID { get { return Request.RequestID; } }
		public MessageStatus Status { get; private set; } = MessageStatus.UnKnown;
		public event EventHandler<IRequest> RequestTimedOut;

		private readonly RoutingFrame Request;
		private readonly IRouter Router;
		private readonly string PublisherID;
		private readonly IKeepAliveMonitor KeepAliveMonitor;
		private IRouter Consumer;

		public RabbitMQRequest(string pPublisherID, IRouter pRouter, RoutingFrame pFrame) {
			PublisherID = pPublisherID;
			Router = pRouter;
			Request = pFrame;

			KeepAliveMonitor = new KeepAliveMonitor(2000);
		}

		public void Send() {
			Status = MessageStatus.Queued;
			if (Request.FrameType == RoutingFrameType.Control) {
				//ToDo: should this be here? - isn't this only for MessageRequests?
				ControlFactory.Processor.Process(Router, Request);
			} else {
				var obj = new RoutingFrame(
					Request.FrameType, 
					Request.MessageType, 
					Request.RequestID, 
					PublisherID, 
					Router.ConnectionID, 
					Request.Data
				);
				Router.Receive(obj.GetBytes());
			}
			KeepAliveMonitor.Start();
			KeepAliveMonitor.TimeOut += KeepAliveMonitor_TimeOut;
			KeepAliveMonitor.UnResponsive += KeepAliveMonitor_UnResponsive;
		}

		public void SetResponse(RoutingFrame pFrame) {
			EndRequest();
			Router.Transmit(pFrame.GetBytes());
		}

		public void SetStatus(IRouter pConsumerRouter, MessageStatus pStatus) {
			if(pStatus == MessageStatus.Busy || pStatus == MessageStatus.Queued) {
				KeepAliveMonitor.Reset();
				Consumer = pConsumerRouter;
			}
			Status = pStatus;
		}

		private void KeepAliveMonitor_TimeOut(object sender, System.EventArgs e) {
			Log.Info($"Request {Request.RequestID} timed out on {Router.ConnectionID}, checking status...");
			KeepAliveMonitor.StartCountDown();

			if (Consumer != null) {
				Task.Run(() => {
					ControlFactory.Processor.Request(new ControlRequest<GetMessageStatusResponse>(
						Consumer,
						new GetMessageStatusRequest(Request.RequestID),
						(GetMessageStatusResponse r) => {
							if (r.IsAlive) {
								KeepAliveMonitor.Reset();
							} else {
								EndRequest();
								RequestTimedOut?.Invoke(this, this);
							}
						}
					));
				});
			}
		}

		private void KeepAliveMonitor_UnResponsive(object sender, System.EventArgs e) {
			EndRequest();
			RequestTimedOut?.Invoke(this, this);
		}

		private void EndRequest() {
			KeepAliveMonitor.TimeOut -= KeepAliveMonitor_TimeOut;
			KeepAliveMonitor.UnResponsive -= KeepAliveMonitor_UnResponsive;
			KeepAliveMonitor.Stop();
		}
	}
}