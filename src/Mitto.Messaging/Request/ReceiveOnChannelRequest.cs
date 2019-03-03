namespace Mitto.Messaging.Request {
	public class ReceiveOnChannelRequest : RequestMessage {
		public string ChannelName { get; set; }
		public string Message { get; set; }

		public ReceiveOnChannelRequest(string pName, string pMessage) {
			ChannelName = pName;
			Message = pMessage;
		}
	}
}