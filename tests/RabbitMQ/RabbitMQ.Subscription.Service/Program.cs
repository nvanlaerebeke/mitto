using Mitto.IMessaging;
using Mitto.Messaging;

namespace RabbitMQ.SubscriptionService {

    internal class Program {
        private static Mitto.Subscription.Service.RabbitMQ.SubscriptionService _service;

        private static void Main() {
            var tmp = typeof(Mitto.Subscription.Messaging.UnSubscribe.ChannelUnSubscribe);
            System.Console.WriteLine(tmp.Name);

            var objMessageProcessor = new MessageProcessor(new RequestManager(), new Mitto.Subscription.Service.RabbitMQ.SubscriptionActionManager());
            Mitto.Config.Initialize(new Mitto.Config.ConfigParams() {
                // RouterProvider = new RouterProvider(new RabbitMQParams() { Hostname = "test.crazyzone.be" })
                MessageProcessor = objMessageProcessor
            });
            _service = new Mitto.Subscription.Service.RabbitMQ.SubscriptionService();
            _service.Start();
        }
    }
}