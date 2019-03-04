using Mitto.IMessaging;
using Mitto.Messaging.Request;

namespace Mitto.Messaging.Response {
    public class EchoResponse: ResponseMessage {
        public EchoResponse() { }
        public EchoResponse(IEchoRequest pMessage, ResponseStatus pStatus) : base(pMessage, pStatus) {
			Message = pMessage.Message;
		}

		public EchoResponse(IEchoRequest pMessage) : base(pMessage, new ResponseStatus()) {
			Message = pMessage.Message;
		}

		public string Message { get; set; }
    }
}