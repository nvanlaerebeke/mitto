using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Mitto.Connection.Websocket.Server;
using WebSocketSharp.Server;

namespace Mitto.Connection.Websocket {
	internal class WebSocketServerWrapper : IWebSocketServer {
		private WebSocketServer _objWebSocket;

		public event EventHandler<IWebSocketBehavior> ClientConnected;

		public void Start(IPAddress pIPAddress, int pPort) {
			StartServer(pIPAddress, pPort);
		}

		public void Start(IPAddress pIPAddress, int pPort, X509Certificate2 pCert) {
			StartServer(pIPAddress, pPort, pCert);
		}

		private void StartServer(IPAddress pIPAddress, int pPort, X509Certificate2 pCert = null) {
			_objWebSocket = new WebSocketServer(pIPAddress, pPort, (pCert != null));
			_objWebSocket.WaitTime = new TimeSpan(0, 0, 30);

			if (pCert != null) {
				_objWebSocket.SslConfiguration.ServerCertificate = pCert;
			}
			_objWebSocket.AddWebSocketService("/", delegate (WebsocketClientWrapper pClient) {
				ClientConnected?.Invoke(this, pClient);
			});
			_objWebSocket.KeepClean = true;
			_objWebSocket.Start();
		}

		public void Stop() {
			_objWebSocket.Stop();
		}
	}
}