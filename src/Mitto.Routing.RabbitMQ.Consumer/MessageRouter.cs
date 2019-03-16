using Mitto.IMessaging;
using Mitto.IRouting;

namespace Mitto.Routing.RabbitMQ.Consumer {
	internal class MessageRouter : IRouter {
		public string ID { get; private set; }

		private readonly Frame _objFrame;
		private SenderQueue _objPublisherQueue;

		private SenderQueue PublisherQueue {
			get {
				if (_objPublisherQueue == null) {
					_objPublisherQueue = new SenderQueue(_objFrame.QueueID);
				}
				return _objPublisherQueue;
			}
		}

		public MessageRouter(string pConsumerID, Frame pFrame) {
			ID = pFrame.ConnectionID;
			_objFrame = pFrame;
		}

		public void Start() {
			MessagingFactory.Processor.Process(this, _objFrame.Data);
		}

		public void Close() { }

		public void Transmit(byte[] pData) {
			var objFrame = new Frame(ID, _objFrame.ConnectionID, pData);
			PublisherQueue.Transmit(objFrame);
		}
	}
}