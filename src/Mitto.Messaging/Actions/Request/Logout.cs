using Mitto.IMessaging;

namespace Mitto.Messaging.Action.Request {
    public class Logout : RequestAction<Messaging.Request.Logout> {
        public Logout(IQueue.IQueue pClient, Messaging.Request.Logout pMessage) : base(pClient, pMessage) { }

        public override ResponseMessage Start() {
            return new Response.ACK(Request, ResponseCode.Success);
        }
    }
}