
using Mitto.IQueue;

namespace Mitto.Queue.RabbitMQ.Consumer {
	/// <summary>
	/// RabbitMQ Consumer Message Handler
	/// 
	/// This is a consumer that reads from the MittoMain queue and processes any requests
	/// 
	/// </summary>
	public class RabbitMQ : RabbitMQBase {
		public override event DataHandler Rx;

		public RabbitMQ() : base("MittoMain") { }

		/// <summary>
		/// Receives a message from the Queue where:
		///     Message.ClientID: Sender Queue
		///     Message.Data: RabbitMQDataMessage
		/// </summary>
		/// <param name="pMessage"></param>
		public override void Receive(Message pMessage) {
			RabbitMQDataMessage objMessage = new RabbitMQDataMessage(pMessage.Data);
			Rx?.Invoke(new Message(objMessage.QueueID, pMessage.Data));
		}

		public override void Transmit(Message pMessage) {
			RabbitMQDataMessage objMessage = new RabbitMQDataMessage("MainMitto", pMessage.ClientID, pMessage.Data);
			this.AddToTxQueue(new Message(objMessage.ClientID, objMessage.GetBytes()));
		}
	}
}