using Mitto.ILogging;
using Mitto.IMessaging;
using Mitto.IRouting;
using Mitto.Subscription.Messaging;
using System;

namespace Mitto.Routing.PassThrough {

    internal class PassThroughSubscriptionRouter<T> : IRouter {
        private readonly ILog Log = LogFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IRouter Router;
        public string SourceID { get { return Router.ConnectionID; } }
        public string DestinationID { get { return Router.ConnectionID; } }

        public string ConnectionID => throw new NotImplementedException();

        public PassThroughSubscriptionRouter(IRouter pRouter) {
            Router = pRouter;
        }

        public void Transmit(byte[] pData) {
            var objRoutingFrame = new RoutingFrame(pData);
            var objMessage = MessagingFactory.Provider.GetMessage(objRoutingFrame.Data);
            var objHandler = MessagingFactory.Provider.GetSubscriptionHandler<T>();

            if (objMessage is SubMessage) {
                objHandler.GetType().GetMethod("Sub").Invoke(objHandler, new object[] { Router, objMessage });
            } else if (objMessage is UnSubMessage) {
                objHandler.GetType().GetMethod("UnSub").Invoke(objHandler, new object[] { Router, objMessage });
            } else if (objMessage is IRequestMessage) {
                objHandler.GetType().GetMethod("Notify").Invoke(objHandler, new object[] { Router, objMessage });
            }
        }

        public void Receive(byte[] pData) {
            throw new NotImplementedException();
        }

        public void Close() {
            throw new NotImplementedException();
        }

        public bool IsAlive(string pRequestID) {
            throw new NotImplementedException();
        }
    }
}