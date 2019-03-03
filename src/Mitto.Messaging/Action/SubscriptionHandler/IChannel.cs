using Mitto.IMessaging;
using Mitto.Messaging.Request;

namespace Mitto.Messaging.Action.SubscriptionHandler {
	public interface IChannel {
		bool Notify(IClient pSender, ISendToChannel pNotifyMessage);
		bool Sub(IClient pClient, Messaging.Subscribe.Channel pMessage);
		bool UnSub(IClient pClient, Messaging.UnSubscribe.Channel pMessage);
	}
}