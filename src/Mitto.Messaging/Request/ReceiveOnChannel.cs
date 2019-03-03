namespace Mitto.Messaging.Request {
	public class ReceiveOnChannel : RequestMessage {
		public string ChannelName { get; set; }
		public string Message { get; set; }

		public ReceiveOnChannel(string pName, string pMessage) {
			ChannelName = pName;
			Message = pMessage;
		}
	}
}