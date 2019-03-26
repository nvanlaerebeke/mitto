using Mitto.IMessaging;
using Mitto.IRouting;
using System;

namespace Mitto.Messaging.Response {
    public class ACKResponse: ResponseMessage {
        public ACKResponse() { }

		public ACKResponse(IRequestMessage pMessage): base(pMessage, new ResponseStatus()) { }

        public ACKResponse(IRequestMessage pMessage, ResponseStatus pStatus) : base(pMessage, pStatus) { }
    }
}
