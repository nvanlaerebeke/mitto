namespace Mitto.Messaging.Request {
	public class SendToChannel : RequestMessage, ISendToChannel {
		public string ChannelName { get; set; }
		public string Message { get; set; }

		public SendToChannel(string pName, string pMessage) {
			ChannelName = pName;
			Message = pMessage;
		}
	}
}