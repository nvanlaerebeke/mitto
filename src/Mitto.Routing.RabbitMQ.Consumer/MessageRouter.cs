using Mitto.IMessaging;
using Mitto.IRouting;

namespace Mitto.Routing.RabbitMQ.Consumer {
	internal class MessageRouter : IRouter {
		public string ConnectionID { get; private set; }

		private readonly RoutingFrame Frame;
		private readonly SenderQueue Publisher;

		public MessageRouter(string pConsumerID, SenderQueue pPublisher, RoutingFrame pFrame) {
			ConnectionID = pConsumerID;
			Publisher = pPublisher;
			Frame = pFrame;
		}

		public void Start() {
			MessagingFactory.Processor.Process(this, Frame.Data);
		}

		public void Close() { }

		public void Receive(byte[] pData) {
			//nothing to do, not applicable for RabbitMQ consumer
		}

		public void Transmit(byte[] pData) {
			var objFrame = new RoutingFrame(
				RoutingFrameType.Messaging,
				Frame.RequestID,
				Consumer.ID,
				Frame.DestinationID,
				new RoutingFrame(pData).Data
			);
			var objMessage = IMessaging.MessagingFactory.Provider.GetMessage(pData);
			Publisher.Transmit(objFrame);
		}

		public bool IsAlive(string pRequestID) {
			return true;
		}
	}
}