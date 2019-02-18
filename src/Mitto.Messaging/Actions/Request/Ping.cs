using Mitto.IMessaging;

namespace Mitto.Messaging.Action.Request {
    public class Ping : RequestAction<Messaging.Request.Ping> {
        public Ping(IQueue.IQueue pClient, Messaging.Request.Ping pMessage) : base(pClient, pMessage) { }

        public override ResponseMessage Start() {
            return new Response.Pong(Request, ResponseCode.Success);
        }
    }
}