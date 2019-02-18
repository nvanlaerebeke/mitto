using Mitto.IMessaging;

namespace Mitto.Messaging.Action {
    public abstract class BaseAction<T> where T: IMessage {
        protected T Request { get; private set; }

        protected IQueue.IQueue Client { private set; get; }

        public BaseAction(IQueue.IQueue pClient, T pRequest) {
            Client = pClient;
            Request = pRequest;
        }
    }
}
