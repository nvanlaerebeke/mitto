namespace Mitto.Messaging.Action {
    public abstract class EventAction<T> : BaseAction<T> where T: EventMessage {
        public EventAction(IQueue.IQueue pClient, T pMessage) : base(pClient, pMessage) { }
        public abstract void Start();
    }
}