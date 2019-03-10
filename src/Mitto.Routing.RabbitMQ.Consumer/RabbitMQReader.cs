using Mitto.IRouting;

namespace Mitto.Routing.RabbitMQ.Consumer {
	public static class RabbitMQReader {
		private static IRouter Router { get; set; }

		public static void Start() {
			Router = new RabbitMQ();
			//Router.Rx += Queue_Rx;
		}
		/// <summary>
		/// ToDo: MessageProcessor should be comming from IMessaging not the Mitto.Messaging.Base
		/// We want to do this to not couple the base assembly in the Queueing, making it implement an 
		/// interface will also make it easier to test
		/// </summary>
		/// <param name="pMessage"></param>
		private static void Queue_Rx(object sender, byte[] data) {
			RabbitMQDataMessage objMsg = new RabbitMQDataMessage(data);
			IMessaging.MessagingFactory.Processor.Process(Router, objMsg.Data);
		}
	}
}