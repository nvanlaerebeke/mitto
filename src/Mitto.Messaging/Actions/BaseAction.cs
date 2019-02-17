using Mitto.IMessaging;

namespace Mitto.Messaging.Action {
    public abstract class BaseAction<T> where T: IMessage {
        protected T Request { get; private set; }

        protected Job Job { private set; get; }

        public BaseAction(Job pJob, T pRequest) {
            Job = pJob;
            Request = pRequest;
        }
    }
}
