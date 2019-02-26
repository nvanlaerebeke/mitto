using Mitto.IConnection;
using Mitto.Utilities;
using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Mitto.Connection.Websocket.Server {
	public class WebsocketServer : IServer {
		private IWebSocketServer _objServer;
		private Action<IClientConnection> _objClientConnected;

		internal WebsocketServer(IWebSocketServer pServer) {
			_objServer = pServer;
			_objServer.ClientConnected += _objServer_ClientConnected;
		}

		private void _objServer_ClientConnected(object sender, IWebSocketBehavior e) {
			_objClientConnected?.Invoke(new Client(e, new KeepAliveMonitor(30000)));
		}

		public void Start(IPAddress pIPAddress, int pPort, Action<IClientConnection> pCallback) {
			_objClientConnected = pCallback;
			_objServer.Start(pIPAddress, pPort);
		}

		public void Start(IPAddress pIPAddress, int pPort, string pCertPath, string pCertPassword, Action<IClientConnection> pCallback) {
			_objClientConnected = pCallback;
			if (!String.IsNullOrEmpty(pCertPath)) {
				if (!System.IO.File.Exists(pCertPath)) {
					throw new System.IO.FileNotFoundException($"{pCertPath} not found");
				}
				_objServer.Start(
					pIPAddress,
					pPort,
					new X509Certificate2(new X509Certificate2(System.IO.File.ReadAllBytes(pCertPath), pCertPassword))
				);
			} else {
				_objServer.Start(pIPAddress, pPort);
			}
		}

		public void Stop() {
			_objServer.ClientConnected -= _objServer_ClientConnected;
			_objServer.Stop();
		}
	}
}