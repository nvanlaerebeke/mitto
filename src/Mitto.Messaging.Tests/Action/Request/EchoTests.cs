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
		public void TestMethod() {
			//Arrange
			var objClient = Substitute.For<IQueue.IQueue>();
			var objMessage = Substitute.For<Messaging.Request.Echo>();
			objMessage.Message = "MyMessage";
			//Act
			var objAction = new Messaging.Action.Request.Echo(objClient, objMessage);
			var objResponse = objAction.Start() as Response.Echo;
			//Assert
			Assert.IsNotNull(objAction);
			Assert.IsNotNull(objResponse);
			Assert.AreEqual("MyMessage", objResponse.Message);
		}
	}
}