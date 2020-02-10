using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Mitto.Connection.Websocket.Wrapper {

    internal class WebSocketClientWrapper : IWebSocketClientWrapper {
        private readonly ClientWebSocket WebSocket;

        public WebSocketClientWrapper()
            => WebSocket = new ClientWebSocket();

        public WebSocketState State
            => WebSocket.State;

        public Task CloseAsync(WebSocketCloseStatus pCloseStatus, string pStatusDescription, CancellationToken pCancellationToken)
            => WebSocket.CloseAsync(pCloseStatus, pStatusDescription, pCancellationToken);

        public Task ConnectAsync(Uri pUri, CancellationToken pCancellationToken)
            => WebSocket.ConnectAsync(pUri, pCancellationToken);

        public Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> pArraySegment, CancellationToken pCancellationToken)
            => WebSocket.ReceiveAsync(pArraySegment, pCancellationToken);

        public Task SendAsync(ArraySegment<byte> pData, WebSocketMessageType pMessageType, bool pEOD, CancellationToken pCancellationToken)
            => WebSocket.SendAsync(pData, pMessageType, pEOD, pCancellationToken);

        public ClientWebSocketOptions Options { get { return WebSocket.Options; } }

        // Destructor - cleanup
        ~WebSocketClientWrapper() {
            try {
                if (WebSocket != null) {
                    WebSocket.Dispose();
                }
            } catch { }
        }
    }
}