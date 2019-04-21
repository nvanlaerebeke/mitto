using Mitto.ILogging;
using Mitto.IMessaging;
using Mitto.IRouting;
using Mitto.Messaging;
using Mitto.Messaging.Response;
using System.Threading;
using System.Threading.Tasks;

namespace Mitto.Subscription.Messaging {

    internal class SubscriptionClient<T> : ISubscriptionClient {
        private readonly ILog Log = LogFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
            ManualResetEvent objBlock = new ManualResetEvent(false);
            var blnResult = false;

            Task.Run(() => {
                MessagingFactory.Processor.Request<ACKResponse>(Router, pMessage, (r) => {
                    blnResult = (r.Status.State == ResponseState.Success);
                    objBlock.Set();
                });
            });

            objBlock.WaitOne(30000);
            return blnResult;
        }

        /*private RoutingFrame GetFrame(IMessage pMessage) {
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
        }*/
    }
}