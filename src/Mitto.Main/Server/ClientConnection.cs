using Mitto.IConnection;
using Mitto.IQueue;
using System;

namespace Mitto {
	public class ClientConnection {
		public event EventHandler<ClientConnection> Disconnected;
		private IClientConnection Connection { get; set; }
		private IQueue.IQueue InternalQueue { get; set; }

		public ClientConnection(IClientConnection pConnection) {
			Connection = pConnection;
			InternalQueue = QueueFactory.Create();

			Connection.Disconnected += Connection_Disconnected;
			Connection.Rx += Connection_Rx;
			InternalQueue.Rx += InternalQueue_Rx;
		}

		private void Connection_Rx(IConnection.IConnection pConnection, byte[] pData) {
			InternalQueue.Transmit(pData);
		}
		private void InternalQueue_Rx(byte[] pData) {
			Connection.Transmit(pData);
		}

		private void Connection_Disconnected(object sender, IConnection.IConnection pConnection) {
			Connection.Disconnected -= Connection_Disconnected;
			Connection.Rx -= Connection_Rx;
			InternalQueue.Rx -= InternalQueue_Rx;

			Disconnected?.Invoke(pConnection, this);
		}
	}
}