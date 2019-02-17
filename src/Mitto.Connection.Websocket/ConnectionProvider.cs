using Mitto.IConnection;

namespace Mitto.Connection.Websocket {
	public class ConnectionProvider: IConnectionProvider {
		public IClient CreateClient() {
			return new Client.WebsocketClient(new WebSocketClientWrapper());
		}

		public IServer CreateServer() {
			return new Server.WebsocketServer(new WebSocketServerWrapper());
		}
	}
}