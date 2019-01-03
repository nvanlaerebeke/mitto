using System.Collections.Generic;
using System;
using IQueue;
using IConnection;

namespace ServerManager {
	internal static class ConnectionManager {
		private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		static Dictionary<string, IClientConnection> _dicClients = new Dictionary<string, IClientConnection>();

		public static void Add(IClientConnection pClient) {
			pClient.Disconnected += PClient_clientDisconnected;
			pClient.Rx += PClient_Rx;
			lock (_dicClients) {
				_dicClients.Add(pClient.ID, pClient);
				Log.Debug("Client Added, " + _dicClients.Count + " clients connected");
			}
		}

		#region Tx & Rx for Connection + Client Disconnect
		private static void PClient_Rx(IConnection.IConnection pClient, byte[] pData) {
			Log.Debug("Received " + pData.Length + " bytes for " + pClient.ID);
			Log.Debug("Adding Data on the Queue to be processed");
			Queue.Transmit(new Message(pClient.ID, pData));
		}

		private static void PClient_clientDisconnected(IConnection.IConnection pClient) {
			lock (_dicClients) {
				if (_dicClients.ContainsKey(pClient.ID)) {
					_dicClients.Remove(pClient.ID);
				}
			}
			Log.Info("Client disconnected, " + _dicClients.Count + " clients connected");
		}
		#endregion

		#region Tx & Rx for Messaging
		private static IQueue.IQueue _objQueue = null;
		private static IQueue.IQueue Queue {
			get {
				if (_objQueue == null) {
					_objQueue = QueueFactory.Get();
					_objQueue.Rx += _objQueue_Rx;
				}
				return _objQueue;
			}
		}
		private static void _objQueue_Rx(Message pMessage) {
			Log.Debug("Received  " + pMessage.Data.Length + " bytes on Queue for " + pMessage.ClientID);
			_dicClients[pMessage.ClientID].Transmit(pMessage.Data);
		}
		#endregion

		/*#region Tx & Rx for Messaging
		private static IMessageProcessor _objMessageProcess = null;

		private static IMessageProcessor MessageProcessor {
			get {
				if (_objMessageProcess == null) {
					_objMessageProcess = MessageProcessorFactory.Get();
					_objMessageProcess.Rx += _objMessageProcess_Rx;
				}
				return _objMessageProcess;
			}
		}

		private static void _objMessageProcess_Rx(Message pMessage) {
			throw new NotImplementedException();
		}
		#endregion*/
	}
}