using Mitto.IMessaging;
using Mitto.IRouting;

namespace Mitto.Routing.RabbitMQ.Consumer {
	internal class MessageRouter : IRouter {
		public string ConnectionID { get; private set; }

		private readonly Frame Frame;
		private readonly SenderQueue Publisher;

		public MessageRouter(string pConsumerID, SenderQueue pPublisher, Frame pFrame) {
			ConnectionID = pConsumerID;
			Publisher = pPublisher;
			Frame = pFrame;
		}

		public void Start() {
			MessagingFactory.Processor.Process(this, Frame.Data);
		}

		public void Close() { }

		public void Transmit(byte[] pData) {
			Publisher.Transmit(
				new Frame(ConnectionID, Frame.ConnectionID, pData)
			);
		}
	}
}