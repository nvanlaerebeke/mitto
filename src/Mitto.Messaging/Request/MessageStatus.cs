namespace Mitto.Messaging.Request {
    public class MessageStatus: RequestMessage {
        public MessageStatus() : base() { }
		public MessageStatus(string pRequestID) : base() {
			RequestID = pRequestID;
		}
		public string RequestID { get; set; }
    }
}