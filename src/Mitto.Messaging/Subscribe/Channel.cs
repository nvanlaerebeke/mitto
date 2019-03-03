namespace Mitto.Messaging.Subscribe {
	public class Channel : SubMessage {
		public string ChannelName { get; set; }
		public Channel(string pName) {
			ChannelName = pName;
		}
	}
}