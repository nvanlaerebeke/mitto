using Mitto.Messaging;

namespace ChatSample.Messaging.Subscribe {
	public class ChatSubscribe : SubMessage {
		public string Channel { get; set; }
		public ChatSubscribe(string pChannel) {
			Channel = pChannel;
		}
	}
}