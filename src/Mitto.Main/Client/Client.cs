using System;
using System.Threading;
using System.Threading.Tasks;
using Mitto.IConnection;
using Mitto.IMessaging;

namespace Mitto {
	/// <summary>
	/// Public interface for the Mitto.Client
	/// 
	/// Class responsible for bridging the internal IQueue, IConnection and the IMessaging
	/// 
	/// Provides:
	///   - events for when the client connects/disconnects
	///   - ability to make requests and receive a response
	///   - ability to send a message (fire and forget)
	/// 
	/// </summary>
	public class Client : IQueue.IQueue {
		public event EventHandler<Client> Connected;
		public event EventHandler<Client> Disconnected;

		public string ID { get { return Connection.ID; } }
		public long CurrentBytesPerSecond {  get { return Connection.CurrentBytesPerSecond; } }

		#region IConnection stuff
		private IConnection.IClient Connection { get; set; }

		public Client() {
			Connection = ConnectionFactory.CreateClient();
			Connection.Connected += ObjClient_Connected;
			Connection.Disconnected += ObjClient_Disconnected;
			Connection.Rx += _objClient_Rx;

			InternalQueue = IQueue.QueueFactory.Create();
			InternalQueue.Rx += InternalQueue_Rx;
		}

		public void ConnectAsync(IClientParams pParams) {
			Connection.ConnectAsync(pParams);
		}

		private void ObjClient_Connected(object sender, IConnection.IClient pClient) {
			Task.Run(() => {
				Connected?.Invoke(sender, this);
			});
		}

		private void ObjClient_Disconnected(object sender, EventArgs e) {
			Close();
		}

		/// <summary>
		/// Closes the connection (IConnection) so that everything gets cleaned up
		/// </summary>
		public void Disconnect() {
			Close();
			Connection.Disconnect();
		}

		private void Close() {
			Connection.Rx -= _objClient_Rx;
			Connection.Connected -= ObjClient_Connected;
			Connection.Disconnected -= ObjClient_Disconnected;

			InternalQueue.Rx -= InternalQueue_Rx;

			Disconnected?.Invoke(this, this);
		}

		/// <summary>
		/// Event handler for when the client receives information (binary data)
		/// Is handled by the MessageProcessor
		/// 
		/// The message processor needs to know the internal communication method that implements the IQueue interface
		/// For the client this is the passthrough (in process handling), for the server this can be rabbitmq f.e.
		/// </summary>
		/// <param name="pClient"></param>
		/// <param name="pData"></param>
		private void _objClient_Rx(object sender, byte[] pData) {
			InternalQueue.Transmit(pData);
		}
		#endregion

		#region Internal Messaging
		private IQueue.IQueue InternalQueue { get; set; }

		/// <summary>
		/// Send what we get on the internal messaging queue to the server
		/// </summary>
		/// <param name="pMessage"></param>
		private void InternalQueue_Rx(object sender, byte[] message) {
			this.Transmit(message);
		}
		#endregion

		#region Request Methods
		public void Request<R>(IRequestMessage pRequest, Action<R> pResponseAction) where R : IResponseMessage {
			MessagingFactory.Processor.Request(this, pRequest, pResponseAction);
		}

		/// <summary>
		/// Runs the Request asynchoniously in a Task
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
		/// Makes a synchronious request
		/// 
		/// Note that this block the application run
		/// It is recommended to use Request<R>(IRequestMessage, Action<R>) instead
		/// </summary>
		/// <typeparam name="R"></typeparam>
		/// <param name="pRequest"></param>
		/// <returns></returns>
		public R Request<R>(IRequestMessage pRequest) where R : IResponseMessage {
			var objTask = RequestAsync<R>(pRequest);
			objTask.Wait();
			return objTask.Result;
		}
		#endregion

		#region Connection Queue implementation (IConnection traffic)
		public event EventHandler<byte[]> Rx;

		/// <summary>
		/// Transmitting data to the server from the internal queue
		/// ToDo: Split off the internal queue functionality and the Client IQueue implementation 
		///       Also making sure it cannot be abused
		/// </summary>
		/// <param name="pMessage"></param>
		public void Transmit(byte[] data) {
			Rx?.Invoke(this, data);
			Connection.Transmit(data);
		}

		/// <summary>
		/// Data from the connection (Receiving) passing it off the internal communication queue
		/// </summary>
		/// <param name="pMessage"></param>
		public void Respond(byte[] pMessage) {
			InternalQueue.Transmit(pMessage);
		}

		public void Receive(byte[] data) {
			Rx?.Invoke(this, data);
		}
		#endregion
	}
}