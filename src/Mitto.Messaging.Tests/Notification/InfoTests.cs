using Mitto.IMessaging;
using Mitto.IRouting;
using NUnit.Framework;

namespace Mitto.Messaging.Tests.Message.Notification {
	[TestFixture]
	public class InfoTests {
		/// <summary>
		/// Test the Info message creation
		/// This means that when creating the message the string is expected in the Message property
		/// </summary>
		[Test]
		public void CreateTest() {
			var objMessage = new Messaging.Notification.InfoNotification("MyMessage");

			Assert.IsNotEmpty(objMessage.ID);
			Assert.AreEqual("InfoNotification", objMessage.Name);
			Assert.AreEqual(MessageType.Notification, objMessage.Type);
			Assert.AreEqual("MyMessage", objMessage.Message);
		}
	}
}