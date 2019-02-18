namespace Mitto.Messaging.Action {
    public abstract class RequestAction<T>: BaseAction<T> where T: RequestMessage {
        public RequestAction(IQueue.IQueue pClient, T pRequest) : base(pClient, pRequest) { }
        public abstract ResponseMessage Start();
    }
}