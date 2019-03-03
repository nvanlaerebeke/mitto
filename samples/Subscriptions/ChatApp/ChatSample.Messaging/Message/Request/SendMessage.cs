namespace ChatSample.Messaging.Request {
	public class SendMessage : Mitto.Messaging.RequestMessage {
		public string Channel { get; set; }
		public string Message { get; set; }
		public SendMessage(string pChannel, string pMessage) {
			Channel = pChannel;
			Message = pMessage;
		}
	}
}
