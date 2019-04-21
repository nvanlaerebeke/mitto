using Mitto.IRouting;
using Mitto.Routing;
using Mitto.Routing.RabbitMQ;
using Mitto.Routing.Request;
using Mitto.Routing.Response;
using System;
using System.Threading;

namespace Mitto.Subscription.Service.RabbitMQ {

    internal class ConsumerRouter : IRouter {
        public string ConnectionID { get { return ConsumerQueue.QueueName; } }

        public readonly SenderQueue ConsumerQueue;

        public ConsumerRouter(SenderQueue pConsumerQueue) {
            ConsumerQueue = pConsumerQueue;
        }

        public void Receive(byte[] pData) {
            throw new NotImplementedException();
        }

        public void Transmit(byte[] pData) {
            var objFrame = new RoutingFrame(pData);
            var objRoutingFrame = new RoutingFrame(
                RoutingFrameType.Messaging,
                objFrame.MessageType,
                objFrame.RequestID,
                SubscriptionService.ID,
                ConsumerQueue.QueueName,
                objFrame.Data
            );
            ConsumerQueue.Transmit(objRoutingFrame);
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
    }
}