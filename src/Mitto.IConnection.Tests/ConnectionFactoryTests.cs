using NSubstitute;
using NUnit.Framework;

namespace Mitto.IConnection.Tests {
	[TestFixture]
	public class ConnectionFactoryTests {
		/// <summary>
		/// Tests the ConnectionFactory
		/// This means that when initializing the Factory the CreateClient/Server methods are called on 
		/// the ConnectionFactory mocked IConnectionProvider
		/// 
		/// Said methods should also return an IServer/IClient object respectively
		/// </summary>
		[Test]
		public void ConfigConnectionTest() {
			var objProvider = Substitute.For<IConnectionProvider>();
			ConnectionFactory.Initialize(objProvider);

			var objClient = ConnectionFactory.CreateClient();
			var objServer = ConnectionFactory.CreateServer();

			Assert.IsInstanceOf<IClient>(objClient);
			Assert.IsInstanceOf<IServer>(objServer);

			objProvider.Received(1).CreateClient();
			objProvider.Received(1).CreateServer();
		}
	}
}
