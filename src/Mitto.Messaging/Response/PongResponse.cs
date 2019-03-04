using Mitto.IMessaging;
using Mitto.Messaging.Request;

namespace Mitto.Messaging.Response {
    public class PongResponse: ResponseMessage {
        public PongResponse() { }
        public PongResponse(PingRequest pMessage) : base(pMessage) { }
    }
}