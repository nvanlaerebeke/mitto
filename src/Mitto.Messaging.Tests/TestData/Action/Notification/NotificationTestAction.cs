using Mitto.Messaging.Action;
using Mitto.Messaging.Tests.TestData.Notification;
using Mitto.IMessaging;

namespace Mitto.Messaging.Tests.TestData.Action.Notification {
	public class NotificationTestAction : NotificationAction<NotificationTestMessage> {
		public NotificationTestAction(IClient pClient, NotificationTestMessage pMessage) : base(pClient, pMessage) { }

		public override void Start() { }
	}
}
