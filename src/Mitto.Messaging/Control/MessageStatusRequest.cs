using Mitto.IMessaging;

namespace Mitto.Messaging.Control {
    public class MessageStatusRequest : RequestMessage, IMessageStatusRequest {
        public MessageStatusRequest() : base(MessageType.Control) { }
		public MessageStatusRequest(string pRequestID) : base(MessageType.Control) {
			RequestID = pRequestID;
		}
		public string RequestID { get; set; }
    }
}