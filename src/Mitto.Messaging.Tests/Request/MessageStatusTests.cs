using Mitto.IMessaging;
using NUnit.Framework;

namespace Mitto.Messaging.Tests.Message.Control {
	[TestFixture]

	public class MessageStatusTests {
		/// <summary>
		/// Test the MessageStatus message creation
		/// This means that when creating the message the Request ID is expected in the RequestID property
		/// </summary>
		[Test]
		public void CreateTest() {
			var objMessage = new Messaging.Control.MessageStatusRequest("MyID");

			Assert.AreEqual("MessageStatusRequest", objMessage.Name);
			Assert.AreEqual(MessageType.Control, objMessage.Type);
			Assert.AreEqual("MyID", objMessage.RequestID);
		}
	}
}
