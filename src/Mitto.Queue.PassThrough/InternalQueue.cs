using Mitto.IQueue;
using System;

namespace Mitto.Queue.PassThrough {
	internal class InternalQueue: IQueue.IQueue {
		public event DataHandler Rx;

		private IQueue.IQueue Queue { get; set; }

		public string ID { get { return Queue.ID; } }

		public InternalQueue(IQueue.IQueue pQueue) {
			Queue = pQueue;
		}

		/// <summary>
		/// Receiving a message in the internal queue means start processing it
		/// Get the configured messageprocessor and process the message
		/// </summary>
		/// <param name="pMessage"></param>
		public void Receive(byte[] pData) {
			IMessaging.MessagingFactory.Processor.Process(this, pData);
		}

		/// <summary>
		/// Transmits a message to the external queue
		/// 
		/// Transmitting here means sending it back to the external queue
		/// and letting that queue do the transmitting
		/// </summary>
		/// <param name="pData"></param>
		public void Transmit(byte[] pData) {
			Queue.Receive(pData);
		}
	}
}