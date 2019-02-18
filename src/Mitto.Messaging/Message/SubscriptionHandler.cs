namespace Mitto.Messaging {
    public abstract class SubscriptionHandler {
        public abstract void Subscribe(IQueue.IQueue pClient);
        public abstract void UnSubscribe(IQueue.IQueue pClient);
        public abstract void Notify(RequestMessage pMessage);
    }
}