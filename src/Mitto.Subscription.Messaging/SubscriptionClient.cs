using Mitto.IMessaging;
using Mitto.IRouting;
using Mitto.Logging;
using Mitto.Messaging;
using Mitto.Messaging.Response;
using System.Threading;
using System.Threading.Tasks;

namespace Mitto.Subscription.Messaging {

    public class SubscriptionClient<T> : ISubscriptionClient where T: ISubscriptionHandler {
        private readonly ILog Log = LoggingFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IRouter Router;
        private readonly IClient Client;

        public SubscriptionClient(IClient pClient) {
            Client = pClient;
            Router = RouterFactory.Provider.GetSubscriptionRouter<T>(pClient.Router);
        }

        public bool Sub(SubMessage pMessage) {
            return Forward(pMessage);
        }

        public bool UnSub(UnSubMessage pMessage) {
            return Forward(pMessage);
        }

        public bool Notify(RequestMessage pMessage) {
            return Forward(pMessage);
        }

        private bool Forward(IRequestMessage pMessage) {
            var objRouter = RouterFactory.Provider.GetSubscriptionRouter<T>(Router);
            objRouter.Transmit(
                new RoutingFrame(
                    RoutingFrameType.Messaging,
                    pMessage.Type,
                    pMessage.ID,
                    Router.ConnectionID,
                    Client.ID,
                    new Frame(
                        pMessage.Type,
                        pMessage.ID,
                        pMessage.Name,
                        MessagingFactory.Converter.GetByteArray(pMessage)
                    ).GetByteArray()
                ).GetBytes()
            );
            return true;
        }
    }
}