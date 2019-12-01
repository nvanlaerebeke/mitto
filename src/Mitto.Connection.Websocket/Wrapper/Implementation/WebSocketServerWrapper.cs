using System;
using System.Net;
using System.Threading.Tasks;

namespace Mitto.Connection.Websocket.Wrapper {
    internal class WebSocketServerWrapper : IWebSocketServerWrapper {
        private readonly HttpListener Listener;

        public WebSocketServerWrapper() => Listener = new HttpListener();

        public bool IsListening => Listener.IsListening;

        public HttpListenerPrefixCollection Prefixes => Listener.Prefixes;

        public void Abort() => Listener.Abort();

        public void Close() => Listener.Close();

        public HttpListenerContext GetContext() => Listener.GetContext();

        public Task<HttpListenerContext> GetContextAsync() => Listener.GetContextAsync();

        public void Start() => Listener.Start();

        public void Stop() => Listener.Stop();
    }
}