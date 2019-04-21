using System;
using System.Threading;
using System.Threading.Tasks;
using Mitto.IConnection;
using Mitto.IMessaging;
using Mitto.IRouting;

namespace Mitto {
	/// <summary>
	/// Public interface for the Mitto.Client
	/// 
	/// Class responsible for bridging the IConnection and MessageProcessor by using an IRouter
	/// 
	/// Provides:
	///   - connection events for when the client connects/disconnects
	///   - routes incomming data to be processed by the messaging
	///   - routes data to be transmitted to be placed on the connection
	/// 
	/// </summary>
	public class Client {
		#region Connection stuff
		/// <summary>
		/// Triggered when the connection has been established
		/// </summary>
		public event EventHandler<Client> Connected;
		/// <summary>
		/// Triggered when the connection gets closed 
		/// </summary>
		public event EventHandler<Client> Disconnected;
		
		
		/// <summary>
		/// Unique identifier for this Client
		/// ToDo: do we need this?
		/// </summary>
		public string ID { get { return Connection.ID; } }
		/// <summary>
		/// Returns the current bytes per second being sent over the connectio.n
		/// 
		/// Note: Only enabled when bandwidth throttling is enabled
		/// </summary>
		public long CurrentBytesPerSecond {  get { return Connection.CurrentBytesPerSecond; } }
		
		/// <summary>
		/// Connection that provides connectivity to the server
		/// </summary>
		private IConnection.IClient Connection { get; set; }
		#endregion

		/// <summary>
		/// Routes any data that is received or needs to be sent
		/// on the Connection
		/// </summary>
		private IRouter Router { get; set; }
		
		/// <summary>
		/// Creates the client that connects to a server
		/// </summary>
		public Client() {
			Connection = ConnectionFactory.CreateClient();
		}

		/// <summary>
		/// Establishes the connection to the server
		/// </summary>
		/// <param name="pParams"></param>
		public void ConnectAsync(IClientParams pParams) {
			// Data handling inside the client:
			//   - Receiving data: deliver the information to the Queue to be processed
			//   - Sending data: put the binary data on the connection
			Router = RouterFactory.Create(Connection);

			Connection.Connected += ObjClient_Connected;
			Connection.Disconnected += ObjClient_Disconnected;
			Connection.ConnectAsync(pParams);
		}

		/// <summary>
		/// Called when the client connection was successful
		/// 
		/// Note, the event is called on a different thread as
		/// not to block anything on the current thread
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="pClient"></param>
		private void ObjClient_Connected(object sender, IConnection.IClient pClient) {
			Task.Run(() => {
				Connected?.Invoke(sender, this);
			});
		}

		/// <summary>
		/// Called when the client connect was unsuccessful or
		/// the client disconnected from the server
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ObjClient_Disconnected(object sender, EventArgs e) {
			Close();
		}

		/// <summary>
		/// Calls the close so that everything gets cleaned up
		/// </summary>
		public void Disconnect() {
			Close();
			Connection.Disconnect();
		}

		/// <summary>
		/// Does the closing/cleanup so nothing is left behind after the client disconnects
		/// </summary>
		private void Close() {
			Connection.Connected -= ObjClient_Connected;
			Connection.Disconnected -= ObjClient_Disconnected;
			if (Router != null) {
				Router.Close();
				Router = null;
			}
			Disconnected?.Invoke(this, this);
		}

		/**
		 * Request methods sending a request to the Server
		 * Requests are managed by the MessageProcessor
		 */

        #region Request Methods

        /// <summary>
        /// Sends a requests and calls the action when the response is received
        ///
        /// This is the recommended way to use Mitto
        /// it is the most efficient resource and speed wise
        ///
        /// The request is passed to the MessageProcessor as it knows how to
        /// handle a requests (ex. how to get the byte[]) and passes it to the
        /// router to be sent over the IClientConnection
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="pRequest"></param>
        /// <param name="pResponseAction"></param>
        public void Request<R>(IRequestMessage pRequest, Action<R> pResponseAction) where R : IResponseMessage {
            MessagingFactory.Processor.Request(Router, pRequest, pResponseAction);
        }

        /// <summary>
        /// Runs the Request asynchronously in a Task
        ///
        /// Note that this uses an extra thread
        /// it is recommended to use Request<R>(IRequestMessage, Action<R>) instead
        /// as that uses much less resources
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="pRequest"></param>
        /// <returns></returns>
        public async Task<R> RequestAsync<R>(IRequestMessage pRequest) where R : IResponseMessage {
            return await Task.Run<R>(() => {
                ManualResetEvent objBlock = new ManualResetEvent(false);
                IResponseMessage objResponse = null;
                Request<R>(pRequest, r => {
                    objResponse = r;
                    objBlock.Set();
                });
                objBlock.WaitOne();
                return (R)objResponse;
            });
        }

        /// <summary>
        /// Makes a synchronous request
        ///
        /// Note that this block the application run
        /// It is recommended to use Request<R>(IRequestMessage, Action<R>) instead
        /// as that does not block the current thread and has a callback action instead
        /// </summary>
        /// <typeparam name="R"></typeparam>
        /// <param name="pRequest"></param>
        /// <returns></returns>
        public R Request<R>(IRequestMessage pRequest) where R : IResponseMessage {
            var objTask = RequestAsync<R>(pRequest);
            objTask.Wait();
            return objTask.Result;
        }

        #endregion Request Methods
    }
}