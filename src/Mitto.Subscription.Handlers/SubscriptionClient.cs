using Mitto.IMessaging;
using Mitto.IRouting;
using Mitto.Messaging;
using Mitto.Messaging.Response;

namespace Mitto.Subscription.Messaging {

    internal class SubscriptionClient<T> : ISubscriptionClient {
        private readonly ISubscriptionRouter Router;
        private readonly IClient Client;

        public SubscriptionClient(IClient pClient) {
            Client = pClient;
            Router = RouterFactory.Provider.GetSubscriptionRouter<T>(pClient.Router);
        }

        public bool Sub(SubMessage pMessage) {
            MessagingFactory.Processor.Request<ACKResponse>(Router, pMessage, (r) => {
            });
            return Router.Sub(GetFrame(pMessage));
        }

        public bool UnSub(UnSubMessage pMessage) {
            return Router.UnSub(GetFrame(pMessage));
        }

        public bool Notify(RequestMessage pMessage) {
            return Router.Notify(GetFrame(pMessage));
        }

        private RoutingFrame GetFrame(IMessage pMessage) {
            return new RoutingFrame(
                RoutingFrameType.Messaging,
                MessageType.Request,
                pMessage.ID,
                Router.SourceID,
                Router.DestinationID,
                new Frame(
                    pMessage.Type,
                    pMessage.ID,
                    pMessage.Name,
                    MessagingFactory.Converter.GetByteArray(pMessage)
                ).GetByteArray()
            );
        }
    }
}