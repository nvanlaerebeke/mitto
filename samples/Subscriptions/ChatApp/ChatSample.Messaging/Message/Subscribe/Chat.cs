namespace ChatSample.Messaging.Subscribe {
	public class Chat : Mitto.Messaging.SubMessage {
		public string Channel { get; set; }
		public Chat(string pChannel) {
			Channel = pChannel;
		}
	}
}