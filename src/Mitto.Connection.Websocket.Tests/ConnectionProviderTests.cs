using Mitto.IConnection;
using NUnit.Framework;

namespace Mitto.Connection.Websocket.Tests {
	[TestFixture]
	public class ConnectionProviderTests {
		[Test]
		public void CreateClient() {
			Assert.IsInstanceOf<IClient>(new WebSocketConnectionProvider().CreateClient());
		}

		[Test]
		public void CreateServer() {
			Assert.IsInstanceOf<IServer>(new WebSocketConnectionProvider().CreateServer());
		}
	}
}
