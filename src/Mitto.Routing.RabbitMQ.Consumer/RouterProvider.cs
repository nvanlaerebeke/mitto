using Mitto.IConnection;
using Mitto.IRouting;

namespace Mitto.Routing.RabbitMQ.Consumer {

    public class RouterProvider : IRouterProvider {

        /// <summary>
        /// Unique identifier for this Provider
        /// </summary>
        public static string ID { get; } = $"Mitto.Consumer.{System.Guid.NewGuid().ToString()}";

        public SenderQueue SubscriptionExchange;

        public RouterProvider(RabbitMQParams pParams) {
            SubscriptionExchange = new SenderQueue(QueueType.SubscriptionMain, $"Mitto.Subscription.Main", true);
        }

        /// <summary>
        /// Creates a Router object that provides communication between the client
        /// and this publisher
        /// </summary>
        /// <param name="pConnection"></param>
        /// <returns></returns>
        public IRouter Create(IClientConnection pConnection) {
            return null;
        }

        public static bool HasRequest(string pRequestID) {
            return false;
        }

        public IRouter GetSubscriptionRouter<T>(IRouter pRouter) {
            var objRouter = pRouter as ConsumerRouter;
            return new SubscriptionRouter(objRouter, SubscriptionExchange);
        }
    }
}