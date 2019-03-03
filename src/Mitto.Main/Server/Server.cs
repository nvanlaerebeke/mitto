using Mitto.IConnection;
using System;
using System.Net;

namespace Mitto {
	/// <summary>
	/// ToDo: Convert the start parameters specific for Websockets in to a internal Config class 
	/// These parameters will then be set uppon initialization
	/// </summary>
	public class Server {
		private IServer _objWsServer;
		private Action<ClientConnection> _objAction;

		public Server() {
			_objWsServer = ConnectionFactory.CreateServer();
		}
		public void Start(IPAddress pIPAddress, int pPort, Action<ClientConnection> pCallback) {
			Start(pIPAddress, pPort, "", "", pCallback);
		}
		public void Start(IPAddress pIPAddress, int pPort, string pCertPath, string pCertPassword, Action<ClientConnection> pCallback) {
			_objAction = pCallback;
			_objWsServer.Start(pIPAddress, pPort, pCertPath, pCertPassword, ClientConnected);
		}
		private void ClientConnected(IClientConnection pClient) {
			_objAction.Invoke(new ClientConnection(pClient));
		}
	}
}