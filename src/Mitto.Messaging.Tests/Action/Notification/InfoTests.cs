using Mitto.IMessaging;
using NSubstitute;
using NUnit.Framework;

namespace Mitto.Messaging.Tests.Action.Notification {
	[TestFixture]
	public class InfoTests {
		/// <summary>
		/// Tests the Info action for the notification message Info
		/// 
		/// Currently the action does nothing, so just testing the constructor
		/// </summary>
		[Test]
		public void TestMethod() {
			//Arrange
			var objClient = Substitute.For<IClient>();
			var objMessage = Substitute.For<Messaging.Notification.InfoNotification>();

			//Act
			var objAction = new Messaging.Action.Notification.InfoNotificationAction(objClient, objMessage);
			objAction.Start();
		}
	}
}