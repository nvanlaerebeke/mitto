
using Mitto.IMessaging;
using Mitto.IRouting;
using System;

namespace Mitto.Routing.RabbitMQ.Consumer {
	/// <summary>
	/// RabbitMQ Consumer
	/// 
	/// Reads data from the main queue and passes it to the IMessageProcessor
	/// for handling. 
	/// Sends any data for the IConnection on the client Queue 
	/// </summary>
	public class RabbitMQ : IRouter {
		private string ID => Guid.NewGuid().ToString();
		/// <summary>
		/// Sending and Listening Queues
		/// </summary>
		private Queue Queue;

		public RabbitMQ() {
			Queue = new Queue(ID, "MittoMain");
			Queue.Rx += Queue_Rx;
		}

		private void Queue_Rx(object sender, Frame e) {
			//Create a router that sends data on the Client Queue
			//var obj = new QueueRouter(e.QueueID, Queue);
			//MessagingFactory.Processor.Process(obj, e.Data);
		}

		public void Close() {
			Queue.Rx -= Queue_Rx;
			Queue.Close();
		}

		public void Transmit(byte[] pMessage) {
			//not implemented
		}
	}
}