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

namespace Mitto.Subscription.RabbitMQ {

    internal class SubscriptionRouter : IRouter, IEquatable<SubscriptionRouter> {
        public string ConnectionID { get; private set; }

        private readonly ReaderQueue ReaderQueue;
        private readonly SenderQueue SenderQueue;
        private readonly Executor Executor;

        public SubscriptionRouter(string pConnectionID, ReaderQueue pReaderQueue, SenderQueue pSenderQueue) {
            ConnectionID = pConnectionID;
            ReaderQueue = pReaderQueue;
            SenderQueue = pSenderQueue;
            Executor = new Executor(this);
        }

        public void Receive(byte[] pData) {
            var objMessage = MessagingFactory.Provider.GetMessage(pData);
            if (objMessage is SubMessage) {
                Executor.Sub(objMessage as SubMessage);
            } else if (objMessage is UnSubMessage) {
                Executor.UnSub(objMessage as UnSubMessage);
            } else if (objMessage is RequestMessage) {
                Executor.Notify(objMessage as RequestMessage);
            }
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
            SenderQueue.Transmit(objRoutingFrame);
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

        public bool Equals(SubscriptionRouter pRouter) {
            return (
                this == pRouter || (
                    ConnectionID.Equals(pRouter.ConnectionID)
                )
            );
        }
    }
}