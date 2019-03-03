using Mitto.IMessaging;
using Mitto.Messaging.Tests.TestData.Request;
using Mitto.Messaging.Tests.TestData.Subscribe;
using Mitto.Messaging.Tests.TestData.UnSubscribe;

namespace Mitto.Messaging.Tests.TestData.Action.SubscriptionHandler {
	public class SubscriptionHandlerTestClass : ISubscriptionHandler<
			TestData.Subscribe.SubscribeTestMessage,
			TestData.UnSubscribe.UnSubscribeTestMessage,
			TestData.Request.RequestTestMessage
	> {
		public bool Notify(IClient pSender, RequestTestMessage pNotifyMessage) {
			return true;
		}

		public bool Sub(IClient pClient, SubscribeTestMessage pMessage) {
			return true;
		}

		public bool UnSub(IClient pClient, UnSubscribeTestMessage pMessage) {
			return true;
		}
	}
}
