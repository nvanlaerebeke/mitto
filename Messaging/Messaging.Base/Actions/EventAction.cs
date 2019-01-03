namespace Messaging.Base.Action {
    public abstract class EventAction<T> : BaseAction<T> where T: EventMessage {
        public EventAction(Job pClient, T pMessage) : base(pClient, pMessage) { }
        public abstract void Start();
    }
}