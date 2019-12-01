using Mitto.IConnection;
using Mitto.Utilities;

namespace Mitto.Connection.Websocket {

    public class WebSocketConnectionProvider : IConnectionProvider {
        public ServerParams ServerConfig { get; set; } = new ServerParams();

        public IClient CreateClient() {
            return new Client.WebsocketClient(new Wrapper.WebSocketClientWrapper(), 512);
        }

        public IServer CreateServer() {
            return new Server.WebSocketServer(new Wrapper.WebSocketServerWrapper(), ServerConfig);
        }
    }
}