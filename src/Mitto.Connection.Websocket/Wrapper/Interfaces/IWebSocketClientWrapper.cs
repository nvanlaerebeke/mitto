using Mitto.IConnection;
using System;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleToAttribute("Mitto.Connection.Websocket.Tests")]

namespace Mitto.Connection.Websocket.Wrapper {

    internal interface IWebSocketClientWrapper {
        WebSocketState State { get; }

        Task ConnectAsync(Uri pUri, CancellationToken pCancellationToken);

        ClientWebSocketOptions Options { get; }

        Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> pArraySegment, CancellationToken pCancellationToken);

        Task CloseAsync(WebSocketCloseStatus pCloseStatus, string pStatusDescription, CancellationToken pCancellationToken);

        Task SendAsync(ArraySegment<byte> pData, WebSocketMessageType pMessageType, bool pEOD, CancellationToken pCancellationToken);
    }
}