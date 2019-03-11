using Mitto.IMessaging;
using Mitto.IRouting;

namespace Mitto.Routing.RabbitMQ.Consumer {
	internal class MessageRouter : IRouter {
		private readonly Frame _objFrame;
		private readonly string _strConsumerID;
		private SenderQueue _objPublisherQueue;

		private SenderQueue PublisherQueue {
			get {
				if(_objPublisherQueue == null) {
					_objPublisherQueue = new SenderQueue(_objFrame.QueueID);
				}
				return _objPublisherQueue;
			}
		}

		public MessageRouter(string pConsumerID, Frame pFrame) {
			_strConsumerID = pConsumerID;
			_objFrame = pFrame;
		}

		public void Start() {
			MessagingFactory.Processor.Process(this, _objFrame.Data);
		}

		public void Close() { }

		public void Transmit(byte[] pData) {
			var objMessage = MessagingFactory.Provider.GetMessage(pData);
			var objFrame = new Frame(_strConsumerID, objMessage.ID, pData);
			PublisherQueue.Transmit(objFrame);
		}
	}
}