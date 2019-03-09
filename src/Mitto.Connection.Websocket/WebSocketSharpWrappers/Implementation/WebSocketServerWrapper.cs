using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Mitto.Connection.Websocket.Server;
using Mitto.ILogging;
using WebSocketSharp.Server;

namespace Mitto.Connection.Websocket {
	internal class WebSocketServerWrapper : IWebSocketServer {
		private readonly ILog Log = LogFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private WebSocketServer _objWebSocket;

		public int ConnectionTimeoutSeconds { get; set; } = 30;

		public event EventHandler<IWebSocketBehavior> ClientConnected;

		public void Start(IPAddress pIPAddress, int pPort) {
			StartServer(pIPAddress, pPort);
		}

		public void Start(IPAddress pIPAddress, int pPort, X509Certificate2 pCert) {
			StartServer(pIPAddress, pPort, pCert);
		}

		private void StartServer(IPAddress pIPAddress, int pPort, X509Certificate2 pCert = null) {
			Log.Info("Starting Websocket server");
			Log.Info($"Listening on {pIPAddress.ToString()}:{pPort}");

			_objWebSocket = new WebSocketServer(pIPAddress, pPort, (pCert != null));
			_objWebSocket.WaitTime = new TimeSpan(0, 0, ConnectionTimeoutSeconds);
			_objWebSocket.Log.Level = WebSocketSharp.LogLevel.Fatal;

			if (pCert != null) {
				Log.Info("Setting up a secure connection...");
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