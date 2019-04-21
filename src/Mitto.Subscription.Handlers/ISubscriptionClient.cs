using Mitto.Messaging;

namespace Mitto.Subscription.Messaging {
    internal interface ISubscriptionClient {
        bool Notify(RequestMessage pMessage);
        bool Sub(SubMessage pMessage);
        bool UnSub(UnSubMessage pMessage);
    }
}