using Mitto.IMessaging;

namespace Mitto.Subscription.IMessaging {

    internal interface ISubscriptionClient {

        bool Notify(IRequestMessage pMessage);

        bool Sub(ISubMessage pMessage);

        bool UnSub(IUnSubMessage pMessage);
    }
}