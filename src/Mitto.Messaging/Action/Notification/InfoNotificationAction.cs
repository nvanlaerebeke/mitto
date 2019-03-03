using Mitto.IMessaging;
using Mitto.Messaging.Notification;

namespace Mitto.Messaging.Action.Notification {
    public class InfoNotificationAction : NotificationAction<InfoNotification> {
        public InfoNotificationAction(IClient pClient, InfoNotification pMessage) : base(pClient, pMessage) { }

		public override void Start() { }
    }
}