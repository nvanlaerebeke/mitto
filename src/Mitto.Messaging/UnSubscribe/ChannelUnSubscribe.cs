namespace Mitto.Messaging.UnSubscribe {
	public class ChannelUnSubscribe : UnSubMessage {
		public string ChannelName { get; set; }

		public ChannelUnSubscribe(string pName) {
			ChannelName = pName;
		}
	}
}