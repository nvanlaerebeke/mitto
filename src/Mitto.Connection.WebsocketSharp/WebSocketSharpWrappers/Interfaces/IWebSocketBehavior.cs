using System;

namespace Mitto.Connection.WebsocketSharp.Server {

    internal interface IWebSocketBehavior {

        event EventHandler<IMessageEventArgs> OnMessageReceived;

        event EventHandler<ICloseEventArgs> OnCloseReceived;

        event EventHandler<IErrorEventArgs> OnErrorReceived;

        string ID { get; }

        void SendAsync(byte[] pData);

        void Close();

        bool Ping();
    }
}