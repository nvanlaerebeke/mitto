namespace Mitto.Messaging.UnSubscribe {
	public class Channel : UnSubMessage {
		public string ChannelName { get; set; }

		public Channel(string pName) {
			ChannelName = pName;
		}
	}
}