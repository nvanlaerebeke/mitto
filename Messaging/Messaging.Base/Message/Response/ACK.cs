using System;

namespace Messaging.Base.Response {
    public class ACK: ResponseMessage{
        public ACK() { }
        public ACK(RequestMessage pMessage, ResponseCode pStatus) : base(pMessage, pStatus) {  }

        public override byte GetCode() {
            return 0x55;
        }
    }
}
