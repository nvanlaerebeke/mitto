using Mitto.IMessaging;
using Mitto.IRouting;
using Mitto.Subscription.Messaging.Tests.TestData.Request;
using Mitto.Subscription.Messaging.Tests.TestData.Subscribe;
using Mitto.Subscription.Messaging.Tests.TestData.UnSubscribe;

namespace Mitto.Subscription.Messaging.Tests.TestData.Action.SubscriptionHandler {

    public class SubscriptionHandlerTestClass : ISubscriptionHandler<
            SubscribeTestMessage,
            UnSubscribeTestMessage,
            RequestTestMessage
    > {

        public bool Notify(IRouter pSender, RequestTestMessage pNotifyMessage) {
            return true;
        }

        public bool Sub(IRouter pClient, SubscribeTestMessage pMessage) {
            return true;
        }

        public bool UnSub(IRouter pClient, UnSubscribeTestMessage pMessage) {
            return true;
        }
    }
}