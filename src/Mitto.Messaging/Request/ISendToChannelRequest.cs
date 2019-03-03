namespace Mitto.Messaging.Request {
	public interface ISendToChannelRequest : IMessaging.IRequestMessage {
		string ChannelName { get; set; }
		string Message { get; set; }
	}
}