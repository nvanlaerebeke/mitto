using Mitto.IConnection;
using Mitto.IRouting;

namespace Mitto.Routing.RabbitMQ.Publisher {

    public class RouterProvider : IRouterProvider {

        /// <summary>
        /// Unique identifier for this Provider
        /// </summary>
        public static string ID { get; } = $"Mitto.Publisher.{System.Guid.NewGuid().ToString()}";

        private static ReaderQueue PublisherQueue;
        private static RouterCache RouterCache;
        private static RequestManager RequestManager;

        public RouterProvider(RabbitMQParams pParams) {
            RequestManager = new RequestManager();
            RouterCache = new RouterCache(new SenderQueue(QueueType.Main, "Mitto.Main", true), RequestManager);

            PublisherQueue = new ReaderQueue(QueueType.Publisher, ID, false);
            PublisherQueue.Rx += PublisherQueue_Rx;
        }

        private void PublisherQueue_Rx(object sender, RoutingFrame pFrame) {
            if (pFrame.FrameType == RoutingFrameType.Messaging) {
                var objRouter = RouterCache.GetByRouterID(pFrame.DestinationID) as Router;
                if (objRouter != null) {
                    objRouter.Transmit(pFrame);
                }
            } else if (pFrame.FrameType == RoutingFrameType.Control) {
                var objRouter = RouterCache.GetByConsumerID(pFrame.SourceID) as ConsumerRouter;
                if (objRouter != null) {
                    objRouter.Receive(pFrame.GetBytes());
                }
            }
        }

        /// <summary>
        /// Creates a Router object that provides communication between the client
        /// and this publisher
        /// </summary>
        /// <param name="pConnection"></param>
        /// <returns></returns>
        public IRouter Create(IClientConnection pConnection) {
            return RouterCache.GetByConnection(pConnection);
        }

        public static bool HasRequest(string pRequestID) {
            return RequestManager.ContainsRequest(pRequestID);
        }

        public IRouter GetSubscriptionRouter<T>(IRouter pRouter) {
            return null;
        }
    }
}