using Mitto.IConnection;
using NSubstitute;
using NUnit.Framework;

namespace Mitto.IRouting.Tests {
	[TestFixture]
	public class RoutingFactoryTests {
		/// <summary>
		/// Tests the QueueFactory initialization
		/// This means that when the Create method is called on the QueueFactory
		/// the methods will be call on the mocked provider that was passed in 
		/// the configuration
		/// 
		/// Said create method should also return an IQueue object
		/// </summary>
		[Test]
		public void ConfigRouterTest() {
			//Arrange
			var objProvider = Substitute.For<IRouterProvider>();
			var objConnection = Substitute.For<IClient>();
			RouterFactory.Initialize(objProvider);
			
			//Act
			var objRouter = RouterFactory.Create(objConnection);

			//Assert
			objProvider.Received(1).Create(Arg.Is(objConnection));
			Assert.IsInstanceOf<IRouter>(objRouter);
		}
	}
}
