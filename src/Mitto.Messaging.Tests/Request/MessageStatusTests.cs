using Mitto.IMessaging;
using NUnit.Framework;

namespace Mitto.Messaging.Tests.Message.Request {
	[TestFixture]

	public class MessageStatusTests {
		/// <summary>
		/// Test the MessageStatus message creation
		/// This means that when creating the message the Request ID is expected in the RequestID property
		/// </summary>
		[Test]
		public void CreateTest() {
			var objMessage = new Messaging.Request.MessageStatusRequest("MyID");
			Assert.AreEqual(MessageType.Request, objMessage.Type);
			Assert.AreEqual("MyID", objMessage.RequestID);
		}
	}
}
