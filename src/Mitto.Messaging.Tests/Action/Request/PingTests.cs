using Mitto.IMessaging;
using NSubstitute;
using NUnit.Framework;

namespace Mitto.Messaging.Tests.Action.Request {
	[TestFixture]
	public class PingTests {
		/// <summary>
		/// Tests the ping action for the requests message ping
		/// This means an Response.Pong message is expected
		/// </summary>
		[Test]
		public void CreateTest() {
			//Arrange
			var objClient = Substitute.For<IClient>();
			var objMessage = Substitute.For<Messaging.Request.Ping>();

			//Act
			var objAction = new Messaging.Action.Request.Ping(objClient, objMessage);
			var objResponse = objAction.Start() as Response.Pong;
			
			//Assert
			Assert.IsNotNull(objAction);
			Assert.IsNotNull(objResponse);
		}
	}
}