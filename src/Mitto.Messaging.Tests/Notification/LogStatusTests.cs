using Mitto.IMessaging;
using Mitto.IRouting;
using NUnit.Framework;

namespace Mitto.Messaging.Tests.Message.Notification {
	[TestFixture]
	public class LogStatusTests {
		/// <summary>
		/// Test the LogStatus creation
		/// </summary>
		[Test]
		public void CreateTest() {
			var objMessage = new Messaging.Notification.LogStatusNotification();

			Assert.IsNotEmpty(objMessage.ID);
			Assert.AreEqual("LogStatusNotification", objMessage.Name);
			Assert.AreEqual(MessageType.Notification, objMessage.Type);
		}
	}
}