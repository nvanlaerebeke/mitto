using Mitto.IConnection;
using System;
using System.Net;

namespace Mitto {
	public class Server {
		private IServer _objWsServer;
		private Action<IClientConnection> _objAction;

		public Server() {
			_objWsServer = ConnectionFactory.CreateServer();
		}
		public void Start(IPAddress pIPAddress, int pPort, Action<IClientConnection> pCallback) {
			Start(pIPAddress, pPort, "", "", pCallback);
		}
		public void Start(IPAddress pIPAddress, int pPort, string pCertPath, string pCertPassword, Action<IClientConnection> pCallback) {
			_objAction = pCallback;
			_objWsServer.Start(pIPAddress, pPort, pCertPath, pCertPassword, ClientConnected);
		}
		private void ClientConnected(IClientConnection pClient) {
			_objAction.Invoke(pClient);
		}
	}
}