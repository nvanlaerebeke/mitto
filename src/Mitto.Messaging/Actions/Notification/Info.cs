namespace Mitto.Messaging.Action.Notification {
    public class Info : NotificationAction<Messaging.Notification.Info> {
        public Info(Job pClient, Messaging.Notification.Info pMessage) : base(pClient, pMessage) { }

        public override void Start() { }
    }
}