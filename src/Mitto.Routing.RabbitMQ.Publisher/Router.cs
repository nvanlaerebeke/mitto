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
	/// </summary>
	internal class Router : IRouter {

		public string ConnectionID { get { return Connection.ID; } }

		public event EventHandler<Router> Disconnected;

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
		/// Creates a router for the IClientConnection
		/// Will listen on MittoMain queue and send data on the queue with 
		/// as name the same as the connection id
		/// 
		/// ToDo: see class summary, no need have the listen/send RabbitMQ queues
		///       here, this can be a single IRouter instead of Queue/IClientConnection
		/// </summary>
		/// <param name="pParams"></param>
		/// <param name="pConnection"></param>
		public Router(SenderQueue pMainQueue, RequestManager pRequestManager, IClientConnection pConnection) {
			RequestManager = pRequestManager;

			Connection = pConnection;
			Connection.Rx += Connection_Rx;
			
			MainQueue = pMainQueue;
		}

		/// <summary>
		/// Called when a frame needs to be processed for this connection
		/// Data is forwared back to the IClientConnection and in case a 
		/// response is expected the request is added to the RequestManager
		/// that will wait for until the respose is received
		///
		/// ToDo: Add KeepAlive so that the Request doesn't infinitely stays in
		/// memory when something goes wrong
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void Process(SenderQueue pOrigin, Frame objFrame) {
			if (
				objFrame.MessageType != IMessaging.MessageType.Response &&
				objFrame.MessageType != IMessaging.MessageType.Notification
			) {
				RequestManager.AddRequest(
					new Request(
						objFrame.ConnectionID,
						objFrame.MessageID,
						pOrigin
					)
				);
			}
			Transmit(objFrame.Data);
		}

		/// <summary>
		/// Triggered when receiving data on the IClientConnection
		/// Creates a Frame and forwards it to the main queue to be
		/// picked up by a consumer/worker node
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Connection_Rx(object sender, byte[] e) {
			var objFrame = new Frame(RouterProvider.ID, ConnectionID, e);
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
			Connection.Rx -= Connection_Rx;
			Disconnected?.Invoke(this, this);
		}
	}
}