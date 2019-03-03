using Mitto.Messaging;

namespace ChatSample.Messaging.Request {
	public class ReceiveMessageRequest : RequestMessage {
		public string Channel { get; set; }
		public string Message { get; set; }
		public ReceiveMessageRequest(string pChannel, string pMessage) {
			Channel = pChannel;
			Message = pMessage;
		}
	}
}
