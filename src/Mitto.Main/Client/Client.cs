using System;
using Mitto.IConnection;
using Mitto.IMessaging;

namespace Mitto {
	public delegate void ClientConnectionHandler(Client pClient);
	/// <summary>
	/// Public interface for the Mitto.Client
	/// 
	/// Class responsible for bridging the internal IQueue, IConnection and the IMessaging
	/// 
	/// Provides:
	///   - events for when the client connects/disconnects
	///   - ability to make requests and receive a response
	///   - ability to send a message (fire and forget)
	///   - easy way to create a new client
	/// 
	/// </summary>
	public class Client : IQueue.IQueue {
		public event ClientConnectionHandler Connected;
		public event ClientConnectionHandler Disconnected;

		#region IConnection stuff
		private IConnection.IClient _objClient;
		public Client() {
			_objClient = ConnectionFactory.CreateClient();
			_objClient.Connected += ObjClient_Connected;
			_objClient.Disconnected += ObjClient_Disconnected;
			_objClient.Rx += _objClient_Rx;
		}

		public void ConnectAsync(string pHostname, int pPort, bool pSecure) {
			_objClient.ConnectAsync(pHostname, pPort, pSecure);
		}

		private void ObjClient_Connected(IConnection.IConnection pClient) {
			Connected?.Invoke(this);
		}
		private void ObjClient_Disconnected(IConnection.IConnection pClient) {
			_objClient.Connected -= ObjClient_Connected;
			_objClient.Disconnected -= ObjClient_Disconnected;

			Disconnected?.Invoke(this);
		}

		/// <summary>
		/// Closes the connection (IConnection) so that everything gets cleaned up
		/// </summary>
		public void Disconnect() {
			_objClient.Disconnect();
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
		private void _objClient_Rx(IConnection.IConnection pClient, byte[] pData) {
			InternalQueue.Transmit(pData);
		}
		#endregion

		#region Internal Messaging (Passthrough)
		private IQueue.IQueue _objQueue;
		private IQueue.IQueue InternalQueue {
			get {
				if (_objQueue == null) {
					_objQueue = IQueue.QueueFactory.Create();
					_objQueue.Rx += _objQueue_Rx;
				}
				return _objQueue;
			}
		}
		/// <summary>
		/// Send what we get on the internal messaging queue to the server
		/// </summary>
		/// <param name="pMessage"></param>
		private void _objQueue_Rx(byte[] pMessage) {
			this.Transmit(pMessage);
		}
		#endregion

		/// <summary>
		/// Sends a request and waits for a response
		/// Handled by the IClient and the Requester knows how to send because this class
		/// implements the IQueue interface (Rx, Tx) and this class knows that it means to send
		/// the data over the wire
		/// </summary>
		/// <typeparam name="R"></typeparam>
		/// <param name="pRequest"></param>
		/// <returns></returns>
		/*public R Request<R>(RequestMessage pRequest) where R : ResponseMessage {
			return Requester.Send<R>(new MessageClient(_objClient.ID, this), pRequest);
		}*/

		public void Request<R>(IRequestMessage pRequest, Action<R> pResponseAction) where R: IResponseMessage {
			MessagingFactory.Processor.Request(this, pRequest, pResponseAction);
		}

		#region Connection Queue implementation (IConnection traffic)
		public event IQueue.DataHandler Rx;

		/// <summary>
		/// Transmitting data to the server from the internal queue
		/// ToDo: Split off the internal queue functionality and the Client IQueue implementation 
		///       Also making sure it cannot be abused
		/// </summary>
		/// <param name="pMessage"></param>
		public void Transmit(byte[] pMessage) {
			Rx?.Invoke(pMessage);
			_objClient.Transmit(pMessage);
		}

		/// <summary>
		/// Data from the connection (Receiving) passing it off the internal communication queue
		/// </summary>
		/// <param name="pMessage"></param>
		public void Respond(byte[] pMessage) {
			InternalQueue.Transmit(pMessage);
		}

		public void Receive(byte[] pMessage) {
			Rx?.Invoke(pMessage);
		}
		#endregion
	}
}