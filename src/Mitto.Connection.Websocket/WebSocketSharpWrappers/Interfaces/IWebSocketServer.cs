using Mitto.Connection.Websocket.Server;
using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Mitto.Connection.Websocket {
	interface IWebSocketServer {
		event EventHandler<IWebSocketBehavior> ClientConnected;

		int ConnectionTimeoutSeconds { get; set; } 

		void Start(IPAddress pIPAddress, int pPort);
		void Start(IPAddress pIPAddress, int pPort, X509Certificate2 pCert);

		void Stop();
	}
}