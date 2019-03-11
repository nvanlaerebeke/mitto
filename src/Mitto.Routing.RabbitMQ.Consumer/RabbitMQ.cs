using Mitto.IRouting;
using System;

namespace Mitto.Routing.RabbitMQ.Consumer {
	/// <summary>
	/// RabbitMQ Consumer
	/// 
	/// Reads data from the main queue and passes it to the IMessageProcessor
	/// for handling. 
	/// Sends any data for the IConnection on the client Queue 
	/// 
	/// ToDo: Change base mitto messaging to not just include the message type but also 
	///       the message id, this way custom Routers etc don't need to conver the binary 
	///       data back to a message before being able to package it
	///       
	/// ToDo: Add an application shutdown handler to close/clean up the queue's
	/// </summary>
	public class RabbitMQ : IRouter {
		/// <summary>
		/// Unique identifier for this Queue
		/// Will be used for communication to this specific Consumer/Worker
		/// </summary>
		public string ID => Guid.NewGuid().ToString();
		
		/// <summary>
		/// Listening Queue for the main Mitto queue
		/// </summary>
		private ReaderQueue MainQueue;

		/// <summary>
		/// Listening Queue for this Consumer
		/// </summary>
		private ReaderQueue ConsumerQueue;

		/// <summary>
		/// Constructor for the RabbitMQ Consumer Queue
		/// 
		/// Listens on MittoMain
		/// 
		/// ToDo: Make the sender Queue optional when creating a queue
		/// </summary>
		public RabbitMQ() {
			MainQueue = new ReaderQueue("MittoMain");
			MainQueue.Rx += MainQueue_Rx;
			ConsumerQueue = new ReaderQueue(ID);
			ConsumerQueue.Rx += ConsumerQueue_Rx;
		}

		/// <summary>
		/// Triggered when messages are read from the main queue
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainQueue_Rx(object sender, Frame e) {
			new MessageRouter(ID, e).Start();
		}

		/// <summary>
		/// Triggered when messages are read from the queue for this worker
		/// 
		/// Responses are forwarded to the RequestManager, any other messages
		/// are handled by the IMessageProcessor
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ConsumerQueue_Rx(object sender, Frame e) {
			new MessageRouter(ID, e).Start();
		}

		/// <summary>
		/// Not used, the RabbitMQ router does not have a single return path
		/// the return queue is depended on the message/connection
		/// </summary>
		/// <param name="pMessage"></param>
		public void Transmit(byte[] pData) {
			//nothing to do
		}

		/// <summary>
		/// Shuts down and cleans up the current worker
		/// </summary>
		public void Close() {
			MainQueue.Rx -= MainQueue_Rx;
			ConsumerQueue.Rx -= ConsumerQueue_Rx;
		}
	}
}