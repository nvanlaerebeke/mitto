using Mitto.IConnection;
using Mitto.IMessaging;
using Mitto.IRouting;
using System;
using System.Linq;

namespace Mitto.Routing.RabbitMQ.Publisher {
	/// <summary>
	/// RabbitMQ Router
	/// Used for publishing byte[] on a queue so a publisher can handle the processing of the data
	/// 
	/// Ideal for deployments where scalability and high availability is key
	/// 
	/// Data received from the IConnection is put on the main queue while
	/// the Queue will listen on the IConnection.ID queue for any data 
	/// that needs to be forwarded to the IConnection
	/// 
	/// ToDo: Convert to one IRouter for the entire consumer
	///       Currently a router is created for every client connection, there really is no
	///       need to do this, there is a need for something that wraps the IClientConnection
	///       but created RabbitMQ queues for each connection is overkill, a single queue per 
	///       consumer plus the main queue is plenty.
	/// </summary>
	public class Router : IRouter {

		public string ID { get { return Connection.ID; } }

		/// <summary>
		/// Unique identifier for this publisher
		/// </summary>
		private readonly string PublisherID;

		/// <summary>
		/// Class that manages all the requests
		/// </summary>
		private readonly RequestManager RequestManager;

		/// <summary>
		/// Client connection
		/// </summary>
		private readonly IClientConnection Connection;

		/// <summary>
		/// Queue that this Consumer sends data to 
		/// for the consumers/workers to process
		/// </summary>
		private readonly SenderQueue MainQueue;

		/// <summary>
		/// Queue that the Consumer listends on to 
		/// get data specific for this connection 
		/// 
		/// This data then gets forwarded back to the IClientConnection
		/// </summary>
		private readonly ReaderQueue PublisherQueue;

		/// <summary>
		/// Creates a router for the IClientConnection
		/// Will listen on MittoMain queue and send data on the queue with 
		/// as name the same as the connection id
		/// 
		/// ToDo: see class summary, no need have the listen/send RabbitMQ queues
		///       here, this can be a single IRouter instead of Queue/IClientConnection
		/// </summary>
		/// <param name="pParams"></param>
		/// <param name="pConnection"></param>
		public Router(SenderQueue pMainQueue, string pPublisherID, IClientConnection pConnection) {
			RequestManager = new RequestManager();

			PublisherID = pPublisherID;

			Connection = pConnection;
			Connection.Rx += Connection_Rx;
			
			MainQueue = pMainQueue;
			PublisherQueue = new ReaderQueue(PublisherID);
			PublisherQueue.Rx += ConnectionQueue_Rx;
		}

		/// <summary>
		/// Triggered when data is received on the Queue for this connection
		/// Data is forwared back to the IClientConnection and in case a 
		/// response is expected the request is added to the RequestManager
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ConnectionQueue_Rx(object sender, Frame e) {
			if(
				e.MessageType != MessageType.Response &&
				e.MessageType != MessageType.Notification
			) {
				RequestManager.AddRequest(e);
			}
			Transmit(e.Data);
		}

		/// <summary>
		/// Triggered when receiving data on the IClientConnection
		/// Creates a Frame and forwards it to the main queue to be
		/// picked up by a consumer/worker node
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Connection_Rx(object sender, byte[] e) {
			var objFrame = new Frame(PublisherID, ID, e);
			if (
				objFrame.MessageType == MessageType.Response
			) {
				RequestManager.SetResponse(objFrame);
			} else {
				MainQueue.Transmit(objFrame);
			}
		}

		/// <summary>
		/// Transmits data back to the IClientConnection
		/// </summary>
		/// <param name="pData"></param>
		public void Transmit(byte[] pData) {
			Connection.Transmit(pData);
		}

		/// <summary>
		/// Shuts down the current router
		/// </summary>
		public void Close() {
			MainQueue.Close();

			Connection.Rx -= Connection_Rx;
			PublisherQueue.Rx -= ConnectionQueue_Rx;
		}
	}
}