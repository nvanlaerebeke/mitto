namespace Mitto.Queue.RabbitMQ.Consumer {
	public static class RabbitMQReader {
		private static IQueue.IQueue Queue { get; set; }

		public static void Start() {
			Queue = new RabbitMQ();
			Queue.Rx += Queue_Rx;
		}
		/// <summary>
		/// ToDo: MessageProcessor should be comming from IMessaging not the Mitto.Messaging.Base
		/// We want to do this to not couple the base assembly in the Queueing, making it implement an 
		/// interface will also make it easier to test
		/// </summary>
		/// <param name="pMessage"></param>
		private static void Queue_Rx(byte[] pMessage) {
			RabbitMQDataMessage objMsg = new RabbitMQDataMessage(pMessage.Data);
			IMessaging.MessagingFactory.Processor.Process(Queue, objMsg.Data);
		}
	}
}