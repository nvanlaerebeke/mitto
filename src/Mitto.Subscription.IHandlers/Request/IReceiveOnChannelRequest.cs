using Mitto.IMessaging;

namespace Mitto.Subscription.IMessaging.Request {
	public interface IReceiveOnChannelRequest : IRequestMessage {
		string ChannelName { get; set; }
		string Message { get; set; }
	}
}