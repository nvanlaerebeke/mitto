using IQueue;
using System;

namespace Queue.RabbitMQ.Publisher {
	/// <summary>
	/// RabbitMQ Message Handler
	/// Used for handling the messages by a separate process by using RabbitMQ
	/// 
	/// Ideal for enterprice deployments where scalability and high availability is key
	/// 
	/// Messages passed over the connection are put on the main MittoMain queue
	/// </summary>
	public class RabbitMQ : RabbitMQBase {
		public override event DataHandler Rx;

		/// <summary>
		/// Publisher Queue that's a layer between the Client connection and internal message handler
		/// Writes to the main Rabbit queue and reads from a queue specific for this queue
		/// Reason for making sure the reader queue is specific is because client connections
		/// are not known by the other Publishers
		/// </summary>
		public RabbitMQ() : base(Guid.NewGuid().ToString()) { }

		/// <summary>
		/// Triggered when Receiving a new byte[] message from the Consumer 
		/// This is data gotten from the Consumers in the form of a RabbitMQDataMessage that contains:
		///     - Sender Queue: the worker name
		///     - ClientID: client connection we need to send the response to
		///     - Data: byte[] that represents the response
		///     
		/// </summary>
		/// <param name="pMessage">The Message received here is from the Consumers, the byte array is a RabbitMQDataMessage packet</param>
		public override void Receive(Message pMessage) {
			RabbitMQDataMessage objMessage = new RabbitMQDataMessage(pMessage.Data);
			Rx?.Invoke(new Message(objMessage.ClientID, objMessage.Data)); // -- in the packet we can find the client id 
		}

		/// <summary>
		/// Here we put any data we received from the connection on the queue to be 
		/// handled by a Consumer
		/// </summary>
		/// <param name="pMessage"></param>
		public override void Transmit(Message pMessage) {
			//Repack the byte array as a rabbitmqdata message so we know the client id in the consumer.
			RabbitMQDataMessage objMessage = new RabbitMQDataMessage(this.ReadQueue, pMessage.ClientID, pMessage.Data);

			//Send the byte array to the MittoMain queue - this is the main processing queue for the application
			//all workers read from this queue and know to what queue to respond because it's part of the RabbitMQDataMessage
			AddToTxQueue(new Message(Config.MainQueue, objMessage.GetBytes()));
		}
	}
}