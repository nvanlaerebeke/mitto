namespace Mitto.Messaging.Request {
    public class EchoRequest : RequestMessage, IEchoRequest {
        public EchoRequest() : base() { }
		public EchoRequest(string pMessage) : base() {
			Message = pMessage;
		}
		public string Message { get; set; }
    }
}