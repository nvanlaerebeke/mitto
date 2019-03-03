using Mitto.IMessaging;
using NSubstitute;
using NUnit.Framework;

namespace Mitto.Messaging.Tests.Message.Response {
	[TestFixture]
	public class ACKTests {
		/// <summary>
		/// Test the ACK response message creation
		/// </summary>
		[Test]
		public void CreateTest() {
			var objMessage = Substitute.For<IRequestMessage>();
			objMessage.ID.Returns("MyID");
			var objACK = new Messaging.Response.ACKResponse(objMessage, ResponseCode.Success);

			Assert.AreEqual("MyID", objACK.ID);
			Assert.AreEqual("ACKResponse", objACK.Name);
			Assert.AreEqual(objACK.Request, objMessage);
			Assert.AreEqual(ResponseCode.Success, objACK.Status);
			Assert.AreEqual(MessageType.Response, objACK.Type);
		}
	}
}