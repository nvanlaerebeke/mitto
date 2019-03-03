using Mitto.Messaging;
namespace ChatSample.Messaging.Request {
	public class SendMessageRequest : RequestMessage {
		public string Channel { get; set; }
		public string Message { get; set; }
		public SendMessageRequest(string pChannel, string pMessage) {
			Channel = pChannel;
			Message = pMessage;
		}
	}
}