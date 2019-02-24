using Mitto.IMessaging;

namespace Mitto.Messaging.Response {
    public class Pong: ResponseMessage {
        public Pong() { }
        public Pong(IMessage pMessage, ResponseCode pStatus) : base(pMessage, pStatus) { }
    }
}