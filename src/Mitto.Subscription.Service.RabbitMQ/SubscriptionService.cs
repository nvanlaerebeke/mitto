using Mitto.IMessaging;
using Mitto.IRouting;
using Mitto.Messaging;
using Mitto.Routing;
using Mitto.Routing.RabbitMQ;
using Mitto.Subscription.Messaging;
using System;

namespace Mitto.Subscription.Service.RabbitMQ {

    public class SubscriptionService {
        public static readonly string ID = $"Mitto.Subscription.{Guid.NewGuid().ToString()}";

        private QueueProvider QueueCache;
        private ReaderQueue ReaderQueue;

        public SubscriptionService() {
        }

        public void Start() {
            QueueCache = new QueueProvider(new RabbitMQParams() { Hostname = "test.crazyzone.be" });
            ReaderQueue = QueueCache.GetReaderQueue(QueueType.SubscriptionMain, ID, false);
            ReaderQueue.Rx += ObjMainReader_Rx;

            //ServiceQueue = QueueCache.GetReaderQueue(QueueType.SubscriptionMain, $"Mitto.Subscription.Main.{ID}", true);

            //var objSubscriptionConsumer = new ReaderQueue(QueueType.Subscription, "Mitto.Subscription." + Guid.NewGuid().ToString(), false);
            //var objMainWriter = new SenderQueue(QueueType.Subscription, "Mitto.Subscription.Main", true);
        }

        private void ObjMainReader_Rx(object sender, RoutingFrame e) {
            if (e.FrameType == RoutingFrameType.Control) {
                var objConsumerRouter = new ConsumerRouter(QueueCache.GetSenderQueue(QueueType.Consumer, e.SourceID, false));
                ControlFactory.Processor.Process(objConsumerRouter, e);
            } else {
                var objPublisherRouter = new PublisherRouter(
                    QueueCache.GetSenderQueue(QueueType.Consumer, e.SourceID, false),
                    ReaderQueue,
                    QueueCache.GetSenderQueue(QueueType.Publisher, e.DestinationID, false)
                );
                MessagingFactory.Processor.Process(objPublisherRouter, e.Data);
            }
        }
    }
}