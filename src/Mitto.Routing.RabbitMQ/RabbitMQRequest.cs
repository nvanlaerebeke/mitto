using Mitto.IRouting;

namespace Mitto.Routing.RabbitMQ {
	public class RabbitMQRequest : IRequest {
		public string ID { get { return Request.RequestID; } }

		public MessageStatus Status { get; private set; } = MessageStatus.UnKnown;

		private readonly RoutingFrame Request;
		private readonly IRouter Router;
		private readonly string PublisherID;

		public RabbitMQRequest(string pPublisherID, IRouter pRouter, RoutingFrame pFrame) {
			PublisherID = pPublisherID;
			Router = pRouter;
			Request = pFrame;
		}

		public void Send() {
			Status = MessageStatus.Queued;
			if (Request.FrameType == RoutingFrameType.Control) {
				ControlFactory.Processor.Process(Router, Request);
			} else {
				var obj = new RoutingFrame(Request.FrameType, Request.RequestID, PublisherID, Router.ConnectionID, Request.Data);
				Router.Receive(obj.GetBytes());
			}
		}

		public void SetResponse(RoutingFrame pFrame) {
			var obj = new RoutingFrame(Request.FrameType, Request.RequestID, PublisherID, Router.ConnectionID, pFrame.Data);
			var objMessage = IMessaging.MessagingFactory.Provider.GetMessage(pFrame.Data);
			Router.Transmit(pFrame.GetBytes());
		}
	}
}
