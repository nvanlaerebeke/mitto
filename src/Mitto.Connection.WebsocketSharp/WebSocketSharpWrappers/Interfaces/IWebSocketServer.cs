using Mitto.Connection.WebsocketSharp.Server;
using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Mitto.Connection.WebsocketSharp {

    internal interface IWebSocketServer {

        event EventHandler<IWebSocketBehavior> ClientConnected;

        int ConnectionTimeoutSeconds { get; set; }

        void Start(IPAddress pIPAddress, int pPort);

        void Start(IPAddress pIPAddress, int pPort, X509Certificate2 pCert);

        void Stop();
    }
}