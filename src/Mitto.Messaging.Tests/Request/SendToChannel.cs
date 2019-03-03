using Mitto.IMessaging;
using NUnit.Framework;

namespace Mitto.Messaging.Tests.Message.Request {
	[TestFixture]
	public class SendToChannelTests {
		/// <summary>
		/// Test the SendToChannel message creation
		/// </summary>
		[Test]
		public void CreateTest() {
			var objMessage = new Messaging.Request.SendToChannelRequest("Channel", "MyMessage");

			Assert.IsNotEmpty(objMessage.ID);
			Assert.AreEqual("SendToChannelRequest", objMessage.Name);
			Assert.AreEqual(MessageType.Request, objMessage.Type);
			Assert.AreEqual("Channel", objMessage.ChannelName);
			Assert.AreEqual("MyMessage", objMessage.Message);
		}
	}
}