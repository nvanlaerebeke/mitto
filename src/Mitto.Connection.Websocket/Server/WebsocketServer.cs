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

		public void Start(IServerParams pParams, Action<IClientConnection> pClientConnectedAction) {
			if (!(pParams is ServerParams objParams)) {
				throw new Exception("Incorrect parameters for Websocket server");
			}

			_objClientConnected = pClientConnectedAction;
			if (!String.IsNullOrEmpty(objParams.CertPath)) {
				if (!System.IO.File.Exists(objParams.CertPath)) {
					throw new System.IO.FileNotFoundException($"{objParams.CertPath} not found");
				}
				_objServer.Start(
					objParams.IP,
					objParams.Port,
					new X509Certificate2(new X509Certificate2(System.IO.File.ReadAllBytes(objParams.CertPath), objParams.CertPassword))
				);
			} else {
				_objServer.Start(objParams.IP, objParams.Port);
			}
		}

		public void Stop() {
			_objServer.ClientConnected -= _objServer_ClientConnected;
			_objServer.Stop();
		}
	}
}