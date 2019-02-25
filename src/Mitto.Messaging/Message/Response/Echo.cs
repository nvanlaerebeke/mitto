using Mitto.IMessaging;

namespace Mitto.Messaging.Response {
    public class Echo: ResponseMessage {
        public Echo() { }
        public Echo(Request.Echo pMessage, ResponseCode pStatus) : base(pMessage, pStatus) {
			Message = pMessage.Message;
		}

		public string Message { get; set; }
    }
}