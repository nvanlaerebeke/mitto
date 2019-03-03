using Mitto.Messaging;
namespace ChatSample.Messaging.UnSubscribe {
	public class ChatUnSubscribe : UnSubMessage {
		public string Channel { get; set; }
		public ChatUnSubscribe(string pChannel) {
			Channel = pChannel;
		}
	}
}