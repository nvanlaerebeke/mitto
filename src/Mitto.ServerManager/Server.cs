using Mitto.IConnection;
using System;
using System.Net;

namespace Mitto.Server {
	public class Server {
		private IServer _objWsServer;
		private Action<Client> _objAction;

		public Server() {
			_objWsServer = ConnectionFactory.CreateServer();
		}
		public void Start(IPAddress pIPAddress, int pPort, Action<Client> pCallback) {
			Start(pIPAddress, pPort, "", "", pCallback);
		}
		public void Start(IPAddress pIPAddress, int pPort, string pCertPath, string pCertPassword, Action<Client> pCallback) {
			_objAction = pCallback;
			_objWsServer.Start(pIPAddress, pPort, pCertPath, pCertPassword, ClientConnected);
		}
		private void ClientConnected(IClientConnection pClient) {
			_objAction.Invoke(new Client(pClient));
		}
	}
}