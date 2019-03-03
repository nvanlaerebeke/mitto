namespace ChatSample.Messaging.Request {
	public class ReceiveMessage : Mitto.Messaging.RequestMessage {
		public string Channel { get; set; }
		public string Message { get; set; }
		public ReceiveMessage(string pChannel, string pMessage) {
			Channel = pChannel;
			Message = pMessage;
		}
	}
}
