using Mitto.IMessaging;
using System;

namespace Mitto.Messaging.Response {
    public class ACKResponse: ResponseMessage{
        public ACKResponse() { }
        public ACKResponse(IRequestMessage pMessage, ResponseCode pCode) : base(pMessage, pCode) {  }
    }
}
