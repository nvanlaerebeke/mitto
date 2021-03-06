﻿using Mitto.IRouting;

namespace Mitto.IMessaging {

    public interface ISubscriptionHandler { }

    public interface ISubscriptionHandler<S, U, N> : ISubscriptionHandler
        where S : IRequestMessage
        where U : IRequestMessage
        where N : IRequestMessage {

        bool Sub(IClient pClient, S pMessage);

        bool UnSub(IClient pClient, U pMessage);

        bool Notify(IClient pSender, N pNotifyMessage);

        bool NotifyAll(N pNotifyMessage);
    }
}