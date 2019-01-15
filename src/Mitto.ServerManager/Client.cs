using Mitto.IConnection;
using Mitto.IQueue;

namespace ServerManager {
	internal class Client {
		private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public event ConnectionHandler Disconnected;
		public IClientConnection Connection { get; private set; }


		public Client(IClientConnection pConnection) {
			Connection = pConnection;
			Connection.Disconnected += Connection_Disconnected;
			Connection.Rx += Connection_Rx;
		}

		private void Connection_Rx(IConnection pConnection, byte[] pData) {
			InternalQueue.Transmit(new Message(pConnection.ID, pData));
		}

		private void Connection_Disconnected(IConnection pConnection) {
			Disconnected?.Invoke(pConnection);
		}

		#region Tx & Rx for Messaging
		private IQueue _objInternalQueue = null;
		private IQueue InternalQueue {
			get {
				if (_objInternalQueue == null) {
					_objInternalQueue = QueueFactory.Get();
					_objInternalQueue.Rx += _objInternalQueue_Rx;
				}
				return _objInternalQueue;
			}
		}
		private void _objInternalQueue_Rx(Message pMessage) {
			Log.Debug("Received  " + pMessage.Data.Length + " bytes on Queue for " + pMessage.ClientID);
			Connection.Transmit(pMessage.Data);
		}
		#endregion
	}
}
