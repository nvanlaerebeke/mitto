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
			var objClient = Substitute.For<IQueue.IQueue>();
			var objMessage = Substitute.For<Messaging.Notification.LogStatus>();

			//Act
			var objAction = new Messaging.Action.Notification.LogStatus(objClient, objMessage);
			objAction.Start();
		}
	}
}