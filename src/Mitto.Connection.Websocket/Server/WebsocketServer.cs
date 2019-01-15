using Mitto.IConnection;
using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using WebSocketSharp.Server;

namespace Mitto.Connection.Websocket.Server {
	public class WebsocketServer : IServer {
		private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private WebSocketServer _objServer;

		public void Start(IPAddress pIPAddress, int pPort, Action<IClientConnection> pCallback) {
            Start(pIPAddress, pPort, "", "", pCallback);
        }

        public void Start(IPAddress pIPAddress, int pPort, string pCertPath, string pCertPassword, Action<IClientConnection> pCallback) {
            Log.Info("Creating new server...");
			Log.Info("Host: " + pIPAddress.ToString());
			Log.Info("Port: " + pPort.ToString());

			_objServer = new WebSocketServer(pIPAddress, pPort, !String.IsNullOrEmpty(pCertPath));

            if (!String.IsNullOrEmpty(pCertPath)) {
				Log.Info("Setting up secure connection using certificate " + pCertPath);
                _objServer.SslConfiguration.ServerCertificate = new X509Certificate2(
                    new X509Certificate2(Convert.FromBase64String(
                        Convert.ToBase64String(System.IO.File.ReadAllBytes(pCertPath)))
                    )
                );
            }

			Client.ClientConnectCallback = pCallback;
            _objServer.AddWebSocketService<Client>("/");
			_objServer.Start();
		}
	}
}