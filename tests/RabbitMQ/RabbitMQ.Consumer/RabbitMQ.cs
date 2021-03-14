using Mitto;
using Mitto.Routing.RabbitMQ;
using Mitto.Routing.RabbitMQ.Consumer;

namespace RabbitMQ.Consumer {

    public static class RabbitMQ {
        private static Mitto.Routing.RabbitMQ.Consumer.Consumer _objConsumer;

        public static void Start() {
            var tmp = typeof(Mitto.Subscription.Messaging.UnSubscribe.ChannelUnSubscribe);
            System.Console.WriteLine(tmp.Name);

            var objProvider = new RouterProvider(new RabbitMQParams() { Hostname = "test.crazyzone.be" });
            Config.Initialize(new Config.ConfigParams() {
                RouterProvider = objProvider
            });
            _objConsumer = new Mitto.Routing.RabbitMQ.Consumer.Consumer(new RabbitMQParams());
        }
    }
}