using Mitto.IRouting;
using System;

namespace Mitto.Routing.RabbitMQ.Consumer {
	internal class QueueRouter : IRouter {
		private readonly Queue Internal;
		public QueueRouter(string pSenderQueue, string pListenQueue) {
			Internal = new Queue(pSenderQueue, pListenQueue);
		}
		public void Close() {
			//nothing to do?
		}

		public void Transmit(byte[] pMessage) {
			Internal.Transmit(new Frame(pMessage));
		}
	}
}
