namespace Mitto.Messaging.Base.Action {
    public abstract class NotificationAction<T> : BaseAction<T> where T: NotificationMessage {
        public NotificationAction(Job pClient, T pMessage) : base(pClient, pMessage) { }
        public abstract void Start();
    }
}