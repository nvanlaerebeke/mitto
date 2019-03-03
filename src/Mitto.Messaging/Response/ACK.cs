using Mitto.IMessaging;
using System;

namespace Mitto.Messaging.Response {
    public class ACK: ResponseMessage{
        public ACK() { }
        public ACK(IRequestMessage pMessage, ResponseCode pCode) : base(pMessage, pCode) {  }
    }
}
