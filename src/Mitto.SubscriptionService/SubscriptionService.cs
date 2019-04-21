using Mitto.IMessaging;
using Mitto.IRouting;
using Mitto.Messaging;
using Mitto.Routing.RabbitMQ;
using Mitto.Subscription.Messaging;
using System;

namespace Mitto.Subscription.RabbitMQ {

    public class SubscriptionService {
        public static readonly string ID = Guid.NewGuid().ToString();
        private readonly RequestManager RequestManager;
        private QueueProvider QueueCache;
        private ReaderQueue ReaderQueue;
        private ReaderQueue ServiceQueue;

        public SubscriptionService() {
            RequestManager = new RequestManager();
        }

        public void Start() {
            QueueCache = new QueueProvider(new RabbitMQParams() { Hostname = "test.crazyzone.be" });
            ReaderQueue = QueueCache.GetReaderQueue(QueueType.SubscriptionMain, $"Mitto.Subscription.{ID}", false);
            ReaderQueue.Rx += ObjMainReader_Rx;

            //ServiceQueue = QueueCache.GetReaderQueue(QueueType.SubscriptionMain, $"Mitto.Subscription.Main.{ID}", true);

            //var objSubscriptionConsumer = new ReaderQueue(QueueType.Subscription, "Mitto.Subscription." + Guid.NewGuid().ToString(), false);
            //var objMainWriter = new SenderQueue(QueueType.Subscription, "Mitto.Subscription.Main", true);
        }

        private void ObjMainReader_Rx(object sender, RoutingFrame e) {
            var objRouter = new SubscriptionRouter(
                e.SourceID,
                ReaderQueue,
                QueueCache.GetSenderQueue(QueueType.Publisher, e.DestinationID, false)
            );
            objRouter.Receive(e.Data);
        }
    }
}