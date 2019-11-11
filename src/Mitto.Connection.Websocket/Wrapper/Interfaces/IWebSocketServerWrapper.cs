using System.Net;
using System.Threading.Tasks;

namespace Mitto.Connection.Websocket.Wrapper {

    internal interface IWebSocketServerWrapper {
        bool IsListening { get; }
        HttpListenerPrefixCollection Prefixes { get; }

        void Abort();

        void Close();

        HttpListenerContext GetContext();

        Task<HttpListenerContext> GetContextAsync();

        void Start();

        void Stop();
    }
}