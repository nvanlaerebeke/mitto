using System;
using System.Runtime.CompilerServices;
using WebSocketSharp;

[assembly: InternalsVisibleToAttribute("Mitto.Connection.Websocket.Tests")]

namespace Mitto.Connection.WebsocketSharp {

    internal interface IWebSocketClient {

        event EventHandler OnOpen;

        event EventHandler<ICloseEventArgs> OnClose;

        event EventHandler<IErrorEventArgs> OnError;

        event EventHandler<IMessageEventArgs> OnMessage;

        int ConnectionTimeoutSeconds { get; set; }
        long CurrentBytesPerSecond { get; }
        WebSocketState ReadyState { get; }

        void ConnectAsync(ClientParams pParams);

        void Close();

        void Send(byte[] pData);

        void SendAsync(byte[] pData);

        bool Ping();
    }
}