using Mitto.IMessaging;
using Mitto.Messaging;

namespace Mitto.Subscription.Messaging {

    public class SubscriptionClient<T> : ISubscriptionClient where T : ISubscriptionHandler {
        private readonly IClient Client;

        public SubscriptionClient(IClient pClient) {
            Client = pClient;
        }

        public bool Sub(SubMessage pMessage) {
            T objHandler = MessagingFactory.Provider.GetSubscriptionHandler<T>();
            return (bool)objHandler.GetType().GetMethod("Sub").Invoke(objHandler, new object[] { Client, pMessage });
        }

        public bool UnSub(UnSubMessage pMessage) {
            T objHandler = MessagingFactory.Provider.GetSubscriptionHandler<T>();
            return (bool)objHandler.GetType().GetMethod("UnSub").Invoke(objHandler, new object[] { Client, pMessage });
        }

        public bool Notify(RequestMessage pMessage) {
            T objHandler = MessagingFactory.Provider.GetSubscriptionHandler<T>();
            return (bool)objHandler.GetType().GetMethod("Notify").Invoke(objHandler, new object[] { Client, pMessage });
        }
    }
}