using System.Collections.Generic;
using Mitto.IConnection;

namespace ServerManager {
	internal static class ConnectionManager {
		static Dictionary<string, Client> _dicClients = new Dictionary<string, Client>();

		public static void Add(IClientConnection pClient) {
			var objClient = new Client(pClient);
			objClient.Disconnected += PClient_clientDisconnected;

			lock (_dicClients) {
				_dicClients.Add(pClient.ID, objClient);
				//Log.Debug("Client " + pClient.ID + " Added, " + _dicClients.Count + " clients connected");
			}
		}

		#region Tx & Rx for Connection + Client Disconnect


		private static void PClient_clientDisconnected(IConnection pClient) {
			lock (_dicClients) {
				if (_dicClients.ContainsKey(pClient.ID)) {
					_dicClients.Remove(pClient.ID);
				}
			}
			//Log.Info("Client " + pClient.ID + " disconnected, " + _dicClients.Count + " clients connected");
		}
		#endregion

		
	}
}