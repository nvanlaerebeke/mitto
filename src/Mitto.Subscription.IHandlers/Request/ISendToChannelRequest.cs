using Mitto.IMessaging;

namespace Mitto.Subscription.IMessaging.Request {
	public interface ISendToChannelRequest : IRequestMessage {
		string ChannelName { get; set; }
		string Message { get; set; }
	}
}