using System;
using WebSocketSharp;

namespace Mitto.Connection.WebsocketSharp {

    internal class WebSocketClientWrapper : IWebSocketClient {
        private WebSocket _objWebSocket;

        public WebSocketState ReadyState {
            get {
                if (_objWebSocket == null) {
                    return WebSocketState.Closed;
                }
                return _objWebSocket.ReadyState;
            }
        }

        public int ConnectionTimeoutSeconds { get; set; } = 30;
        public long CurrentBytesPerSecond { get { return (_objWebSocket == null) ? 0 : _objWebSocket.CurrentBytesPerSecond; } }

        public event EventHandler OnOpen;

        public event EventHandler<ICloseEventArgs> OnClose;

        public event EventHandler<IErrorEventArgs> OnError;

        public event EventHandler<IMessageEventArgs> OnMessage;

        public WebSocketClientWrapper() {
        }

        public void Close() {
            if (_objWebSocket != null) {
                _objWebSocket.OnOpen -= _objWebSocket_OnOpen;
                _objWebSocket.OnClose -= _objWebSocket_OnClose;
                _objWebSocket.OnError -= _objWebSocket_OnError;
                _objWebSocket.OnMessage -= _objWebSocket_OnMessage;

                if (
                    _objWebSocket.ReadyState == WebSocketState.Open ||
                    _objWebSocket.ReadyState == WebSocketState.Connecting
                ) {
                    try {
                        _objWebSocket.Close();
                    } catch { }
                }
                _objWebSocket = null;
            }
        }

        /// <summary>
        /// Connects to the websocket server in an asynchonious way
        /// </summary>
        /// <param name="pUrl"></param>
        public void ConnectAsync(ClientParams pParams) {
            _objWebSocket = new WebSocket(String.Format(((pParams.Secure) ? "wss" : "ws") + "://{0}:{1}/", pParams.Hostname, pParams.Port)) {
                WaitTime = new TimeSpan(0, 0, ConnectionTimeoutSeconds),
                EmitOnPing = true,
                EnableRedirection = true,
                MaxBytesPerSecond = pParams.MaxBytePerSecond
            };
            _objWebSocket.Log.Level = LogLevel.Fatal;

            if (!String.IsNullOrEmpty(pParams.Proxy.URL)) {
                _objWebSocket.SetProxy(pParams.Proxy.URL, pParams.Proxy.UserName, pParams.Proxy.Password);
            }

            _objWebSocket.OnOpen += _objWebSocket_OnOpen;
            _objWebSocket.OnClose += _objWebSocket_OnClose;
            _objWebSocket.OnError += _objWebSocket_OnError;
            _objWebSocket.OnMessage += _objWebSocket_OnMessage;

            _objWebSocket.ConnectAsync();
        }

        /// <summary>
        /// Raises the OnMessage event when receiving messages
        /// Note that this creates a new thread (this is to prevent deadlocks in callbacks)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _objWebSocket_OnMessage(object sender, MessageEventArgs e) {
            OnMessage?.Invoke(sender, new MessageEventArgWrapper(e));
        }

        /// <summary>
        /// Raises the OnError event when receiving an error
        /// Note that this creates a new thread (this is to prevent deadlocks in callbacks)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _objWebSocket_OnError(object sender, ErrorEventArgs e) {
            Close();
            OnError?.Invoke(sender, new ErrorEventArgWrapper(e));
        }

        /// <summary>
        /// Raises the OnClose event when the connection closes
        /// Note that this creates a new thread (this is to prevent deadlocks in callbacks)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _objWebSocket_OnClose(object sender, CloseEventArgs e) {
            Close();
            OnClose?.Invoke(sender, new CloseEventArgWrapper(e));
        }

        /// <summary>
        /// Raises the OnOpen event when a connection is established
        /// Note that this creates a new thread (this is to prevent deadlocks in callbacks)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _objWebSocket_OnOpen(object sender, EventArgs e) {
            OnOpen?.Invoke(sender, e);
        }

        /// <summary>
        /// Sends the byte[] to the websocket server
        /// </summary>
        /// <param name="pData"></param>
        public void Send(byte[] pData) {
            _objWebSocket.Send(pData);
        }

        /// <summary>
        /// Sends the byte[] to the websocket server
        /// </summary>
        /// <param name="pData"></param>
        public void SendAsync(byte[] pData) {
            _objWebSocket.SendAsync(pData, (r) => {
                if (!r) {
                    throw new Exception("Send Failed");
                }
            });
        }

        /// <summary>
        /// Sends a ping over the connection to see if it is still alive
        ///
        /// Note: Blocks until a response is received
        /// </summary>
        /// <returns></returns>
        public bool Ping() {
            return _objWebSocket.Ping();
        }
    }
}