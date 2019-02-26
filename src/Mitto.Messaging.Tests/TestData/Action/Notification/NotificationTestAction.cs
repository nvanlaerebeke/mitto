using Mitto.Messaging.Action;
using Mitto.Messaging.Tests.TestData.Notification;

namespace Mitto.Messaging.Tests.TestData.Action.Notification {
	public class NotificationTestAction : NotificationAction<NotificationTestMessage> {
		public NotificationTestAction(IQueue.IQueue pClient, NotificationTestMessage pMessage) : base(pClient, pMessage) { }

		public override void Start() { }
	}
}
