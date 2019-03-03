using Mitto.IMessaging;
using Mitto.Messaging.Request;
using Mitto.Messaging.Response;

namespace Mitto.Messaging.Action.Request {
    public class PingRequestAction : RequestAction<PingRequest, PongResponse> {
        public PingRequestAction(IClient pClient, PingRequest pMessage) : base(pClient, pMessage) { }

        public override IResponseMessage Start() {
            return new Response.PongResponse(Request);
        }
    }
}