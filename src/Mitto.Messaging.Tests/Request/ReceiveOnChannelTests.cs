using Mitto.IMessaging;
using NUnit.Framework;

namespace Mitto.Messaging.Tests.Message.Request {
	[TestFixture]
	public class ReceiveOnChannelTests {
		/// <summary>
		/// Test the ReceiveOnChannel message creation
		/// </summary>
		[Test]
		public void CreateTest() {
			var objMessage = new Messaging.Request.ReceiveOnChannelRequest("Channel", "MyMessage");

			Assert.IsNotEmpty(objMessage.ID);
			Assert.AreEqual("ReceiveOnChannelRequest", objMessage.Name);
			Assert.AreEqual(MessageType.Request, objMessage.Type);
			Assert.AreEqual("Channel", objMessage.ChannelName);
			Assert.AreEqual("MyMessage", objMessage.Message);
		}
	}
}
