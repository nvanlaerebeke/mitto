using Mitto.IMessaging;
using Mitto.IRouting;

namespace Mitto.Routing.RabbitMQ.Consumer {
	internal class MessageRouter : IRouter {
		public string ConnectionID { get; private set; }

		private readonly RabbitMQFrame RabbitMQFrame;
		private readonly RoutingFrame Frame;
		private readonly SenderQueue Publisher;

		public MessageRouter(string pConsumerID, SenderQueue pPublisher, RabbitMQFrame pFrame) {
			ConnectionID = pConsumerID;
			Publisher = pPublisher;
			RabbitMQFrame = pFrame;
			Frame = new RoutingFrame(pFrame.Data);
		}

		public void Start() {
			MessagingFactory.Processor.Process(this, new RoutingFrame(RabbitMQFrame.Data).Data);
		}

		public void Close() { }

		public void Transmit(byte[] pData) {
			var objRoutingFrame = new RoutingFrame(RoutingFrameType.Messaging, Frame.ConnectionID, pData);
			var objRabbitMQFrame = new RabbitMQFrame(RabbitMQFrameType.Messaging, ConnectionID, objRoutingFrame.GetBytes());
			Publisher.Transmit(objRabbitMQFrame);
		}

		public bool IsAlive(string pRequestID) {
			return true;
		}
	}
}