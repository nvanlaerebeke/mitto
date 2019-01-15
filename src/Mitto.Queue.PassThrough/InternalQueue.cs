using Mitto.IQueue;

namespace Mitto.Queue.PassThrough {
	internal class InternalQueue: IQueue.IQueue {
		public event DataHandler Rx;

		IQueue.IQueue _objQueue;
		public InternalQueue(PassThrough pQueue) {
			_objQueue = pQueue;
		}

		public void Receive(Message pMessage) {
			Messaging.Base.MessageProcessor.Process(this, pMessage);
		}

		public void Transmit(Message pMessage) {
			_objQueue.Receive(pMessage);
		}
	}
}
