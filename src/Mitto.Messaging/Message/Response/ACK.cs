using Mitto.IMessaging;
using System;

namespace Mitto.Messaging.Response {
    public class ACK: ResponseMessage{
        public ACK() { }
        public ACK(IMessage pMessage, ResponseCode pStatus) : base(pMessage, pStatus) {  }
    }
}
