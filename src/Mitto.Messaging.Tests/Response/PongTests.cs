using Mitto.IMessaging;
using Mitto.Messaging.Request;
using NSubstitute;
using NUnit.Framework;

namespace Mitto.Messaging.Tests.Message.Response {
	[TestFixture]
	public class PongTests {
		/// <summary>
		/// Test the Pong response message creation
		/// </summary>
		[Test]
		public void CreateTest() {
			var objMessage = Substitute.For<PingRequest>();
			objMessage.ID = "MyID";

			var obj = new Messaging.Response.PongResponse(objMessage);

			Assert.AreEqual("MyID", obj.ID);
			Assert.AreEqual("PongResponse", obj.Name);
			Assert.AreEqual(ResponseState.Success, obj.Status.State);
			Assert.AreEqual(MessageType.Response, obj.Type);
		}
	}
}