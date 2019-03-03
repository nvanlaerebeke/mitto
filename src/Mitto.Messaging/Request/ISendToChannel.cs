namespace Mitto.Messaging.Request {
	public interface ISendToChannel : IMessaging.IRequestMessage {
		string ChannelName { get; set; }
		string Message { get; set; }
	}
}