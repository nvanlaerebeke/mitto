namespace Messaging.Base.Action.Notification {
    public class Info : NotificationAction<Messaging.Base.Notification.Info> {
        public Info(Job pClient, Messaging.Base.Notification.Info pMessage) : base(pClient, pMessage) { }

        public override void Start() {
        }
    }
}