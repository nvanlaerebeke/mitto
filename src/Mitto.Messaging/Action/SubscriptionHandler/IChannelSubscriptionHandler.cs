using Mitto.IMessaging;
using Mitto.Messaging.Request;

namespace Mitto.Messaging.Action.SubscriptionHandler {
	public interface IChannelSubscriptionHandler {
		bool Notify(IClient pSender, ISendToChannelRequest pNotifyMessage);
		bool Sub(IClient pClient, Messaging.Subscribe.ChannelSubscribe pMessage);
		bool UnSub(IClient pClient, Messaging.UnSubscribe.ChannelUnSubscribe pMessage);
	}
}