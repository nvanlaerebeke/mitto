namespace Mitto.Messaging.Base.Action {
    public abstract class RequestAction<T>: BaseAction<T> where T: RequestMessage {
        public RequestAction(Job pClient, T pRequest) : base(pClient, pRequest) { }
        public abstract ResponseMessage Start();
    }
}