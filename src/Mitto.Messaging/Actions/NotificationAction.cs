namespace Mitto.Messaging.Action {
    public abstract class NotificationAction<T> : BaseAction<T> where T: NotificationMessage {
        public NotificationAction(IQueue.IQueue pClient, T pMessage) : base(pClient, pMessage) { }
        public abstract void Start();
    }
}