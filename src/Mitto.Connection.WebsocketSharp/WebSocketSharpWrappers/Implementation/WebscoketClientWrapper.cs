using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Mitto.Connection.WebsocketSharp.Server {

    internal class WebsocketClientWrapper : WebSocketBehavior, IWebSocketBehavior {

        public event EventHandler<IMessageEventArgs> OnMessageReceived;

        public event EventHandler<ICloseEventArgs> OnCloseReceived;

        public event EventHandler<IErrorEventArgs> OnErrorReceived;

        protected override void OnOpen() {
            base.EmitOnPing = true;
            base.OnOpen();
        }

        protected override void OnMessage(MessageEventArgs e) {
            base.OnMessage(e);
            OnMessageReceived?.Invoke(this, new MessageEventArgWrapper(e));
        }

        protected override void OnClose(CloseEventArgs e) {
            base.OnClose(e);
            OnCloseReceived?.Invoke(this, new CloseEventArgWrapper(e));
        }

        protected override void OnError(ErrorEventArgs e) {
            base.OnError(e);
            OnErrorReceived?.Invoke(this, new ErrorEventArgWrapper(e));
        }

        public void SendAsync(byte[] pData) {
            base.SendAsync(pData, (r) => { });
        }

        public new void Close() {
            base.CloseAsync();
        }

        public bool Ping() {
            return (Context != null) ? Context.WebSocket.Ping() : false;
        }
    }
}