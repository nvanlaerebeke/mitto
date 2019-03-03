using Mitto.IMessaging;

namespace Mitto.Messaging.Response {
    public class EchoResponse: ResponseMessage {
        public EchoResponse() { }
        public EchoResponse(Request.EchoRequest pMessage, ResponseCode pStatus) : base(pMessage, pStatus) {
			Message = pMessage.Message;
		}

		public EchoResponse(Request.EchoRequest pMessage) : base(pMessage, ResponseCode.Success) {
			Message = pMessage.Message;
		}

		public string Message { get; set; }
    }
}