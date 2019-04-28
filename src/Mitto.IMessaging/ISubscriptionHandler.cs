using Mitto.IRouting;

namespace Mitto.IMessaging {

    public interface ISubscriptionHandler { }

    public interface ISubscriptionHandler<S, U, N> : ISubscriptionHandler
        where S : IRequestMessage
        where U : IRequestMessage
        where N : IRequestMessage {

        bool Sub(IRouter pClient, S pMessage);

        bool UnSub(IRouter pClient, U pMessage);

        bool Notify(IRouter pSender, N pNotifyMessage);
        bool NotifyAll(N pNotifyMessage);
    }
}