using Mitto.IMessaging;

namespace Mitto.Messaging.Action.Request {
    public class Ping : RequestAction<Messaging.Request.Ping> {
        public Ping(IClient pClient, Messaging.Request.Ping pMessage) : base(pClient, pMessage) { }

        public override IResponseMessage Start() {
            return new Response.Pong(Request);
        }
    }
}