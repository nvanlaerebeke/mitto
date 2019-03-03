using Mitto.IMessaging;

namespace Mitto.Messaging.Response {
    public class PongResponse: ResponseMessage {
        public PongResponse() { }
        public PongResponse(IRequestMessage pMessage) : base(pMessage, ResponseCode.Success) { }
    }
}