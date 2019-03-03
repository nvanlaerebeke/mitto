using Mitto.IMessaging;
using NSubstitute;
using NUnit.Framework;

namespace Mitto.Messaging.Tests.Action.Request {
	[TestFixture]
	public class EchoTests {
		/// <summary>
		/// Tests the Echo action for the requests message echo
		/// This means an Response.Echo message is expected with as
		/// message the string given in the requestmessage
		/// </summary>
		[Test]
		public void CreateTest() {
			//Arrange
			var objClient = Substitute.For<IClient>();
			var objMessage = Substitute.For<Messaging.Request.EchoRequest>();
			objMessage.Message = "MyMessage";
			//Act
			var objAction = new Messaging.Action.Request.EchoRequestAction(objClient, objMessage);
			var objResponse = objAction.Start() as Response.EchoResponse;
			//Assert
			Assert.IsNotNull(objAction);
			Assert.IsNotNull(objResponse);
			Assert.AreEqual("MyMessage", objResponse.Message);
		}
	}
}