using Mitto.IMessaging;

namespace Mitto.Messaging.Action.Request {
    public class Echo : RequestAction<Messaging.Request.Echo>{
        public Echo(IQueue.IQueue pClient, Messaging.Request.Echo pMessage) : base(pClient, pMessage) { }

        public IResponseMessage Start() {
            return new Response.Echo(Request, IMessaging.ResponseCode.Success);
        }
    }
}