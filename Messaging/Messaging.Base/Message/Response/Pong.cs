using System;
namespace Messaging.Base.Response {
    public class Pong: ResponseMessage {
        public Pong() { }
        public Pong(RequestMessage pMessage, ResponseCode pStatus) : base(pMessage, pStatus) { }

        public override byte GetCode() {
            return 0x56;
        }
    }
}