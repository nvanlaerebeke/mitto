using Mitto.IMessaging;
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
			var objMessage = Substitute.For<IRequestMessage>();
			objMessage.ID.Returns("MyID");

			var obj = new Messaging.Response.PongResponse(objMessage);

			Assert.AreEqual("MyID", obj.ID);
			Assert.AreEqual("PongResponse", obj.Name);
			Assert.AreEqual(obj.Request, objMessage);
			Assert.AreEqual(ResponseCode.Success, obj.Status);
			Assert.AreEqual(MessageType.Response, obj.Type);
		}
	}
}