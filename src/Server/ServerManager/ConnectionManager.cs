using System.Collections.Generic;
using IQueue;
using IConnection;

namespace ServerManager {
	internal static class ConnectionManager {
		private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		static Dictionary<string, Client> _dicClients = new Dictionary<string, Client>();

		public static void Add(IClientConnection pClient) {
			var objClient = new Client(pClient);
			objClient.Disconnected += PClient_clientDisconnected;

			lock (_dicClients) {
				_dicClients.Add(pClient.ID, objClient);
				Log.Debug("Client " + pClient.ID + " Added, " + _dicClients.Count + " clients connected");
			}
		}

		#region Tx & Rx for Connection + Client Disconnect


		private static void PClient_clientDisconnected(IConnection.IConnection pClient) {
			lock (_dicClients) {
				if (_dicClients.ContainsKey(pClient.ID)) {
					_dicClients.Remove(pClient.ID);
				}
			}
			Log.Info("Client " + pClient.ID + " disconnected, " + _dicClients.Count + " clients connected");
		}
		#endregion

		
	}
}