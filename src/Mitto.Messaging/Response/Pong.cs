using Mitto.IMessaging;

namespace Mitto.Messaging.Response {
    public class Pong: ResponseMessage {
        public Pong() { }
        public Pong(IRequestMessage pMessage) : base(pMessage, ResponseCode.Success) { }
    }
}