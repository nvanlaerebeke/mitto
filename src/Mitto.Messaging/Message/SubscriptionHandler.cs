namespace Mitto.Messaging {
    public abstract class SubscriptionHandler {
        public abstract void Subscribe(Job pClient);
        public abstract void UnSubscribe(Job pClient);
        public abstract void Notify(RequestMessage pMessage);
    }
}