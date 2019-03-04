namespace Mitto.Messaging.Request {
    public class MessageStatusRequest : RequestMessage, IMessageStatusRequest {
        public MessageStatusRequest() : base() { }
		public MessageStatusRequest(string pRequestID) : base() {
			RequestID = pRequestID;
		}
		public string RequestID { get; set; }
    }
}