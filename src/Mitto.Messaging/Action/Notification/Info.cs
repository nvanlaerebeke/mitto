using Mitto.IMessaging;

namespace Mitto.Messaging.Action.Notification {
    public class Info : NotificationAction<Messaging.Notification.Info> {
        public Info(IClient pClient, Messaging.Notification.Info pMessage) : base(pClient, pMessage) { }

        public override void Start() { }
    }
}