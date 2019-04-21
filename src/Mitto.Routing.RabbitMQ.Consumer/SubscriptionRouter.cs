using Mitto.IRouting;
using System;

namespace Mitto.Routing.RabbitMQ.Consumer {

    /// <summary>
    /// ToDo: Make notify/sub/unsub a request with a true/false response
    /// </summary>
    internal class SubscriptionRouter : ISubscriptionRouter, IRouter {
        private readonly ConsumerRouter Router;
        private readonly SenderQueue Exchange;

        public string SourceID { get { return Router.Request.DestinationID; } }
        public string DestinationID { get { return Router.Publisher.QueueName; } }

        public SubscriptionRouter(ConsumerRouter pRouter, SenderQueue pExchange) {
            Router = pRouter;
            Exchange = pExchange;
        }

        public string ID => throw new NotImplementedException();

        public bool Notify(RoutingFrame pFrame) {
            Exchange.Transmit(GetFrame(pFrame));
            return true;
        }

        public bool Sub(RoutingFrame pFrame) {
            Exchange.Transmit(GetFrame(pFrame));
            return true;
        }

        public bool UnSub(RoutingFrame pFrame) {
            Exchange.Transmit(GetFrame(pFrame));
            return true;
        }

        private RoutingFrame GetFrame(RoutingFrame pFrame) {
            return new RoutingFrame(
                pFrame.FrameType,
                pFrame.MessageType,
                pFrame.RequestID,
                SourceID,
                DestinationID,
                pFrame.Data
            );
        }

        public string ConnectionID => throw new NotImplementedException();

        public void Transmit(byte[] pData) {
            Exchange.Transmit(GetFrame(new RoutingFrame(pData)));
        }

        public void Receive(byte[] pData) {
            //throw new NotImplementedException();
        }

        public void Close() {
            //throw new NotImplementedException();
        }

        public bool IsAlive(string pRequestID) {
            //throw new NotImplementedException();
            return true;
        }
    }
}