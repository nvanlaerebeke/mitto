using Mitto.IConnection;
using Mitto.Utilities;

namespace Mitto.Connection.Websocket {
	public class ConnectionProvider: IConnectionProvider {
		public IClient CreateClient() {
			return new Client.WebsocketClient(new WebSocketClientWrapper(), new KeepAliveMonitor(30000));
		}

		public IServer CreateServer() {
			return new Server.WebsocketServer(new WebSocketServerWrapper());
		}
	}
}