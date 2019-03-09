using Mitto.IConnection;
using NUnit.Framework;

namespace Mitto.Connection.Websocket.Tests {
	[TestFixture]
	public class ConnectionProviderTests {
		[Test]
		public void CreateClient() {
			Config.Initialize();
			Assert.IsInstanceOf<IClient>(new WebSocketConnectionProvider().CreateClient());
		}

		[Test]
		public void CreateServer() {
			Config.Initialize();
			Assert.IsInstanceOf<IServer>(new WebSocketConnectionProvider().CreateServer());
		}
	}
}
