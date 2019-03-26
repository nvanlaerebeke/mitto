using Mitto.IConnection;
using Mitto.IRouting;
using Mitto.Routing.Request;
using Mitto.Routing.Response;
using System;
using System.Threading;

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
		public readonly RequestManager RequestManager;

		/// <summary>
		/// Client connection
		/// </summary>
		private readonly IClientConnection Connection;
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
			MainQueue = pMainQueue;
			RequestManager = pRequestManager;

			Connection = pConnection;
			Connection.Rx += Connection_Rx;
		}

		/// <summary>
		/// Triggered when receiving data on the IClientConnection
		/// Creates a Frame and forwards it to the main queue to be
		/// picked up by a consumer/worker node
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Connection_Rx(object sender, byte[] e) {
			var obj = new RabbitMQRequest(RouterProvider.ID, this, new RoutingFrame(e));
			RequestManager.Send(obj);
		}

		public void Receive(byte[] pData) {
			MainQueue.Transmit(new RoutingFrame(pData));
		}

		/// <summary>
		/// Transmits data back to the IClientConnection
		/// </summary>
		/// <param name="pData"></param>
		public void Transmit(byte[] pData) {
			Connection.Transmit(pData);
		}

		public void Transmit(RoutingFrame pFrame) {
			RequestManager.Receive(pFrame);
		}

		/// <summary>
		/// Shuts down the current router
		/// </summary>
		public void Close() {
			Connection.Rx -= Connection_Rx;
			Disconnected?.Invoke(this, this);
		}

		public bool IsAlive(string pRequestID) {
			var blnIsAlive = false;
			ManualResetEvent objWait = new ManualResetEvent(false);

			ControlFactory.Processor.Request(new ControlRequest<GetMessageStatusResponse>(
				this,
				new GetMessageStatusRequest(pRequestID),
				(GetMessageStatusResponse r) => {
					blnIsAlive = r.IsAlive;
					objWait.Set();
				}
			));

			objWait.WaitOne(15);
			return blnIsAlive;
		}
	}
}