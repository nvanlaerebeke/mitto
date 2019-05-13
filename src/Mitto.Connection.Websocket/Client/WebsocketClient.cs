using Mitto.IConnection;
using Mitto.Logging;
using Mitto.Utilities;
using System;
using System.Runtime.CompilerServices;
using WebSocketSharp;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

/// <summary>
/// Class that represents the WebSocket Client in Mitto
/// Provides functionality to communicate with a WebSocket server
/// </summary>
namespace Mitto.Connection.Websocket.Client {

    public class WebsocketClient : IClient {
        private readonly ILog Log = LoggingFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string ID { get; private set; } = Guid.NewGuid().ToString();

        private IWebSocketClient _objWebSocketClient;
        private IKeepAliveMonitor _objKeepAliveMonitor;
        public long CurrentBytesPerSecond { get { return (_objWebSocketClient == null) ? 0 : _objWebSocketClient.CurrentBytesPerSecond; } }

        public event EventHandler<IClient> Connected;

        public event EventHandler<IConnection.IConnection> Disconnected;

        public event EventHandler<byte[]> Rx;

        public static int FragmentLength {
            get { return WebSocket.FragmentLength; }
            set { WebSocket.FragmentLength = value; }
        }

        internal WebsocketClient(IWebSocketClient pWebSocket, IKeepAliveMonitor pKeepAliveMonitor) {
            _objWebSocketClient = pWebSocket;
            _objKeepAliveMonitor = pKeepAliveMonitor;
        }

        #region Constructor & Connecting

        public void ConnectAsync(IClientParams pParams) {
            if (!(pParams is ClientParams objParams)) {
                Log.Error("Incorrect parameters for WebSocket client");
                throw new Exception("Incorrect parameters for WebSocket client");
            }
            Log.Info($"Connecting {ID} to {objParams.Hostname}:{objParams.Port}");

            _objWebSocketClient.ConnectionTimeoutSeconds = objParams.ConnectionTimeoutSeconds;
            _objKeepAliveMonitor.SetInterval(objParams.ConnectionTimeoutSeconds);

            _objWebSocketClient.OnOpen += Connection_OnOpen;
            _objWebSocketClient.OnClose += Connection_OnClose;
            _objWebSocketClient.OnError += Connection_OnError;
            _objWebSocketClient.OnMessage += Connection_OnMessage;

            _objWebSocketClient.ConnectAsync(objParams);

            _objKeepAliveMonitor.TimeOut += _objKeepAliveMonitor_TimeOut;
            _objKeepAliveMonitor.UnResponsive += _objKeepAliveMonitor_UnResponsive;
        }

        private void Close() {
            Log.Info($"Closing connection: {ID}");

            _objKeepAliveMonitor.TimeOut -= _objKeepAliveMonitor_TimeOut;
            _objKeepAliveMonitor.UnResponsive -= _objKeepAliveMonitor_UnResponsive;

            _objWebSocketClient.OnOpen -= Connection_OnOpen;
            _objWebSocketClient.OnClose -= Connection_OnClose;
            _objWebSocketClient.OnError -= Connection_OnError;
            _objWebSocketClient.OnMessage -= Connection_OnMessage;

            if (
                _objWebSocketClient.ReadyState != WebSocketState.Closing &&
                _objWebSocketClient.ReadyState != WebSocketState.Closed
            ) {
                _objWebSocketClient.Close();
            }
            _objKeepAliveMonitor.Stop();
        }

        private void _objKeepAliveMonitor_TimeOut(object sender, EventArgs e) {
            Log.Debug($"Connection timeout: {ID}, pinging...");
            _objKeepAliveMonitor.StartCountDown();
            if (_objWebSocketClient.Ping()) {
                _objKeepAliveMonitor.Reset();
                Log.Debug($"Pong received: {ID}");
            }
        }

        private void _objKeepAliveMonitor_UnResponsive(object sender, EventArgs e) {
            Log.Info($"Connection {ID} lost, closing...");
            this.Disconnect();
        }

        #endregion Constructor & Connecting

        #region WebSocket Event Handlers

        private void Connection_OnOpen(object sender, EventArgs e) {
            Log.Info($"Connection {ID} connected");

            //Start the sending queue before we raise the event
            //The thread must be running before we say the client is 'connected(ready)'
            _objKeepAliveMonitor.Start();
            Connected?.Invoke(this, this);
        }

        private void Connection_OnError(object sender, IErrorEventArgs e) {
            Log.Error($"Connection error for {ID}: {e.Message}");
            this.Close();
            Disconnected?.Invoke(this, this);
        }

        private void Connection_OnClose(object sender, ICloseEventArgs e) {
            Log.Info($"Connection {ID} closed");
            this.Close();
            Disconnected?.Invoke(this, this);
        }

        private void Connection_OnMessage(object sender, IMessageEventArgs e) {
            _objKeepAliveMonitor.Reset();
            if (e.IsText) {
                Log.Debug($"Text received on {ID}");
                var data = System.Text.Encoding.UTF8.GetBytes(e.Data);
                Rx?.Invoke(this, data);
            } else if (e.IsPing) { // -- do nothing, keep-alive is handled in this class
                Log.Debug($"Ping received on {ID}");
            } else if (e.IsBinary) {
                Log.Debug($"Data received on {ID}");
                Rx?.Invoke(this, e.RawData);
            }
        }

        #endregion WebSocket Event Handlers

        #region Client Methods

        public void Disconnect() {
            this.Close();
            Disconnected?.Invoke(this, this);
        }

        /// <summary>
        /// Transmit adds the data to be transfered to the queue
        /// </summary>
        /// <param name="pData"></param>
        public void Transmit(byte[] pData) {
            _objWebSocketClient.SendAsync(pData);
        }

        #endregion Client Methods
    }
}