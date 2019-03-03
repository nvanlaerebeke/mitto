namespace ChatSample.Messaging.UnSubscribe {
	public class Chat : Mitto.Messaging.UnSubMessage {
		public string Channel { get; set; }
		public Chat(string pChannel) {
			Channel = pChannel;
		}
	}
}