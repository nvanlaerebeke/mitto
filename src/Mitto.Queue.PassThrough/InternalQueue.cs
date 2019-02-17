using Mitto.IQueue;

namespace Mitto.Queue.PassThrough {
	internal class InternalQueue: IQueue.IQueue {
		public event DataHandler Rx;

		internal IQueue.IQueue _objQueue;
		public InternalQueue(IQueue.IQueue pQueue) {
			_objQueue = pQueue;
		}

		/// <summary>
		/// Receiving a message in the internal queue means start processing it
		/// Get the configured messageprocessor and process the message
		/// </summary>
		/// <param name="pMessage"></param>
		public void Receive(Message pMessage) {
			IMessaging.MessagingFactory.Processor.Process(this, pMessage);
		}

		/// <summary>
		/// Transmits a message to the external queue
		/// 
		/// Transmitting here means sending it back to the external queue
		/// and letting that queue do the transmitting
		/// </summary>
		/// <param name="pMessage"></param>
		public void Transmit(Message pMessage) {
			_objQueue.Receive(pMessage);
		}
	}
}