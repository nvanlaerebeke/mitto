using Mitto.IConnection;
using Mitto.IQueue;
using System;

namespace Mitto {
	public class ClientConnection {
		public event EventHandler<ClientConnection> Disconnected;
		public IClientConnection Connection { get; private set; }

		public string ID {
			get {
				return Connection.ID;
			}
		}

		public ClientConnection(IClientConnection pConnection) {
			Connection = pConnection;
			Connection.Disconnected += Connection_Disconnected;
			Connection.Rx += Connection_Rx;
		}

		private void Connection_Rx(IConnection.IConnection pConnection, byte[] pData) {
			InternalQueue.Transmit(pData);
		}

		private void Connection_Disconnected(IConnection.IConnection pConnection) {
			Disconnected?.Invoke(pConnection, this);
		}

		#region Tx & Rx for Messaging
		private IQueue.IQueue _objInternalQueue = null;
		private IQueue.IQueue InternalQueue {
			get {
				if (_objInternalQueue == null) {
					_objInternalQueue = QueueFactory.Create();
					_objInternalQueue.Rx += _objInternalQueue_Rx;
				}
				return _objInternalQueue;
			}
		}
		private void _objInternalQueue_Rx(byte[] pData) {
			Connection.Transmit(pData);
		}
		#endregion
	}
}
