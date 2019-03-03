using Mitto.IMessaging;
using NSubstitute;
using NUnit.Framework;

namespace Mitto.Messaging.Tests.Action.Notification {
	[TestFixture]
	public class LogStatusTests {
		/// <summary>
		/// Tests the LogStatus action for the notification message LogStatus
		/// 
		/// Currently the action does nothing, so just testing the constructor
		/// </summary>
		[Test]
		public void TestMethod() {
			//Arrange
			var objClient = Substitute.For<IClient>();
			var objMessage = Substitute.For<Messaging.Notification.LogStatusNotification>();

			//Act
			var objAction = new Messaging.Action.Notification.LogStatusNotificationAction(objClient, objMessage);
			objAction.Start();
		}
	}
}