using ILogging;
using Logging;
using Mitto.IConnection;
using Mitto.Utilities;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Mitto.Connection.Websocket.Server {

    public class WebsocketServer : IServer {
        private readonly ILog Log = LogFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IWebSocketServer _objServer;
        private Action<IClientConnection> _objClientConnected;
        private ServerParams _objParams;

        internal WebsocketServer(IWebSocketServer pServer) {
            _objServer = pServer;
            _objServer.ClientConnected += _objServer_ClientConnected;
        }

        private void _objServer_ClientConnected(object sender, IWebSocketBehavior e) {
            _objClientConnected?.Invoke(new Client(e, new KeepAliveMonitor(_objParams.ConnectionTimeoutSeconds)));
        }

        public void Start(IServerParams pParams, Action<IClientConnection> pClientConnectedAction) {
            if (!(pParams is ServerParams objParams)) {
                Log.Error("Incorrect parameters for Websocket server");
                throw new Exception("Incorrect parameters for Websocket server");
            }
            _objParams = objParams;
            _objServer.ConnectionTimeoutSeconds = _objParams.ConnectionTimeoutSeconds;

            _objClientConnected = pClientConnectedAction;
            if (!String.IsNullOrEmpty(_objParams.CertPath)) {
                if (!System.IO.File.Exists(_objParams.CertPath)) {
                    Log.Error($"{_objParams.CertPath} not found");
                    throw new System.IO.FileNotFoundException($"{_objParams.CertPath} not found");
                }
                _objServer.Start(
                    _objParams.IP,
                    _objParams.Port,
                    new X509Certificate2(new X509Certificate2(System.IO.File.ReadAllBytes(_objParams.CertPath), _objParams.CertPassword))
                );
            } else {
                _objServer.Start(_objParams.IP, _objParams.Port);
            }
        }

        public void Stop() {
            _objServer.ClientConnected -= _objServer_ClientConnected;
            _objServer.Stop();
        }
    }
}