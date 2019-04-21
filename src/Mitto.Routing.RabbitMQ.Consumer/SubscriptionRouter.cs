using Mitto.IRouting;
using Mitto.Routing.Request;
using Mitto.Routing.Response;
using System;
using System.Threading;

namespace Mitto.Routing.RabbitMQ.Consumer {

    /// <summary>
    /// ToDo: Make notify/sub/unsub a request with a true/false response
    /// </summary>
    internal class SubscriptionRouter : IRouter {
        public string ConnectionID { get { return Exchange.QueueName; } }

        private readonly ConsumerRouter Router;
        private readonly SenderQueue Exchange;

        //public string SourceID { get { return Router.Request.DestinationID; } }
        //public string DestinationID { get { return Router.Publisher.QueueName; } }

        public SubscriptionRouter(ConsumerRouter pRouter, SenderQueue pExchange) {
            Router = pRouter;
            Exchange = pExchange;
        }

        /*private RoutingFrame GetFrame(RoutingFrame pFrame) {
            return new RoutingFrame(
                pFrame.FrameType,
                pFrame.MessageType,
                pFrame.RequestID,
                RouterProvider.ID,
                DestinationID,
                pFrame.Data
            );
        }*/

        public void Transmit(byte[] pData) {
            var objRoutingFrame = new RoutingFrame(pData);
            var objNewFrame = new RoutingFrame(
                objRoutingFrame.FrameType,
                objRoutingFrame.MessageType,
                objRoutingFrame.RequestID,
                Consumer.ID,
                Router.Publisher.QueueName,
                objRoutingFrame.Data
            );
            Exchange.Transmit(objNewFrame);
        }

        public void Receive(byte[] pData) {
            throw new NotImplementedException();
        }

        public void Close() {
            throw new NotImplementedException();
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