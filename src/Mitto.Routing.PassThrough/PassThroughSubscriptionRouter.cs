using Mitto.ILogging;
using Mitto.IMessaging;
using Mitto.IRouting;
using System;

namespace Mitto.Routing.PassThrough {

    internal class PassThroughSubscriptionRouter<T> : ISubscriptionRouter {
        private readonly ILog Log = LogFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IRouter Router;
        public string SourceID { get { return Router.ConnectionID; } }
        public string DestinationID { get { return Router.ConnectionID; } }

        public PassThroughSubscriptionRouter(IRouter pRouter) {
            Router = pRouter;
        }

        public bool Notify(RoutingFrame pFrame) {
            try {
                var objMessage = MessagingFactory.Provider.GetMessage(pFrame.Data);
                var objHandler = MessagingFactory.Provider.GetSubscriptionHandler<T>();
                return (bool)objHandler.GetType().GetMethod("Notify").Invoke(objHandler, new object[] { Router, objMessage });
            } catch (Exception ex) {
                Log.Error(ex);
                return false;
            }
        }

        public bool Sub(RoutingFrame pFrame) {
            try {
                var objMessage = MessagingFactory.Provider.GetMessage(pFrame.Data);
                var objHandler = MessagingFactory.Provider.GetSubscriptionHandler<T>();
                return (bool)objHandler.GetType().GetMethod("Sub").Invoke(objHandler, new object[] { Router, objMessage });
            } catch (Exception ex) {
                Log.Error(ex);
                return false;
            }
        }

        public bool UnSub(RoutingFrame pFrame) {
            try {
                var objMessage = MessagingFactory.Provider.GetMessage(pFrame.Data);
                var objHandler = MessagingFactory.Provider.GetSubscriptionHandler<T>();
                return (bool)objHandler.GetType().GetMethod("UnSub").Invoke(objHandler, new object[] { Router, objMessage });
            } catch (Exception ex) {
                Log.Error(ex);
                return false;
            }
        }
    }
}