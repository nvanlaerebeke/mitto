namespace Mitto.Messaging.Request {
    public class EchoRequest: RequestMessage {
        public EchoRequest() : base() { }
		public EchoRequest(string pMessage) : base() {
			Message = pMessage;
		}
		public string Message { get; set; }
    }
}