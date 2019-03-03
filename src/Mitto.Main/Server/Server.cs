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

		/// <summary>
		/// Starts the Server connection 
		/// </summary>
		/// <param name="pParams">Parameters for the server</param>
		/// <param name="pAction">Action that will be run when a client connects</param>
		public void Start(ServerParams pParams, Action<ClientConnection> pAction) {
			_objAction = pAction;
			pParams.ClientConnected = ClientConnected;
			_objWsServer.Start(pParams);
		}
		private void ClientConnected(IClientConnection pClient) {
			_objAction.Invoke(new ClientConnection(pClient));
		}
	}
}