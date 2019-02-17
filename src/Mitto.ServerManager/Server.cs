using Mitto.IConnection;
using System.Net;

namespace ServerManager {
	public class Server {
		private IServer _objWsServer;
		public Server() {
			_objWsServer = ConnectionFactory.CreateServer();
		}
		public void Start(IPAddress pIPAddress, int pPort) {
			Start(pIPAddress, pPort, "", "");
		}
		public void Start(IPAddress pIPAddress, int pPort, string pCertPath, string pCertPassword) {
			_objWsServer.Start(pIPAddress, pPort, pCertPath, pCertPassword, ClientConnected);
		}
		private void ClientConnected(IClientConnection pClient) {
			ConnectionManager.Add(pClient);
		}
	}
}