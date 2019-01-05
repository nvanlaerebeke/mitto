using Messaging.Base;

namespace Queue.RabbitMQ.Consumer {
	public static class RabbitMQReader {
		private static IQueue.IQueue Queue { get; set; }

		public static void Start() {
			Queue = new RabbitMQ();
			Queue.Rx += Queue_Rx;
		}

		private static void Queue_Rx(IQueue.Message pMessage) {
			RabbitMQDataMessage objMsg = new RabbitMQDataMessage(pMessage.Data);
			MessageProcessor.Process(Queue, new IQueue.Message(objMsg.QueueID, objMsg.Data));
		}
	}
}