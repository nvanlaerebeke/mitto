using Mitto.IMessaging;
using Mitto.IRouting;
using Mitto.Messaging;
using Mitto.Routing;
using Mitto.Routing.RabbitMQ;
using Mitto.Routing.Request;
using Mitto.Routing.Response;
using Mitto.Subscription.Messaging;
using System;
using System.Threading;

namespace Mitto.Subscription.Service.RabbitMQ {

    internal class PublisherRouter : IRouter, IEquatable<PublisherRouter> {
        public string ConnectionID { get { return ConsumerQueue.QueueName; } }

        public string SourceID { get { return ReaderQueue.QueueName; } }
        public string DestinationID { get { return PublisherQueue.QueueName; } }
        public readonly SenderQueue ConsumerQueue;
        public readonly SenderQueue PublisherQueue;

        private readonly ReaderQueue ReaderQueue;

        private readonly Executor Executor;

        public PublisherRouter(SenderQueue pConsumerQueue, ReaderQueue pReaderQueue, SenderQueue pPublisherQueue) {
            ConsumerQueue = pConsumerQueue;
            PublisherQueue = pPublisherQueue;
            ReaderQueue = pReaderQueue;
            Executor = new Executor(this);
        }

        public void Receive(byte[] pData) {
        }

        public void Transmit(byte[] pData) {
            var objFrame = new RoutingFrame(pData);
            var objRoutingFrame = new RoutingFrame(
                RoutingFrameType.Messaging,
                objFrame.MessageType,
                objFrame.RequestID,
                ReaderQueue.QueueName,
                ConnectionID,
                objFrame.Data
            );
            PublisherQueue.Transmit(objRoutingFrame);
        }

        public void Close() {
            //nothing to do
        }

        public bool IsAlive(string pRequestID) {
            var blnIsAlive = false;
            ManualResetEvent objWait = new ManualResetEvent(false);

            ControlFactory.Processor.Request(new ControlRequest<GetMessageStatusResponse>(
                this,
                new GetMessageStatusRequest(pRequestID),
                (GetMessageStatusResponse r) => {
                    blnIsAlive = r.IsAlive;
                    objWait.Set();
                }
            ));

            objWait.WaitOne(5000);
            return blnIsAlive;
        }

        public bool Equals(PublisherRouter pRouter) {
            return (
                this == pRouter || (
                    ConnectionID.Equals(pRouter.ConnectionID)
                )
            );
        }
    }
}