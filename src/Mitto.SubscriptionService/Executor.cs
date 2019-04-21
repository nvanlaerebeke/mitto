using Mitto.ILogging;
using Mitto.IMessaging;
using Mitto.IRouting;
using Mitto.Messaging;
using Mitto.Subscription.Messaging;
using System;

namespace Mitto.Subscription.RabbitMQ {

    internal class Executor {
        private ILog Log { get { return LogFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType); } }
        private readonly IRouter Router;

        public Executor(IRouter pRouter) {
            Router = pRouter;
        }

        public bool Sub(SubMessage pMessage) {
            try {
                var objHandler = MessagingFactory.Provider.GetSubscriptionHandler(pMessage);
                return (bool)objHandler.GetType().GetMethod("Sub").Invoke(objHandler, new object[] { Router, pMessage });
            } catch (Exception ex) {
                Log.Error(ex);
                return false;
            }
        }

        public bool UnSub(UnSubMessage pMessage) {
            try {
                var objHandler = MessagingFactory.Provider.GetSubscriptionHandler(pMessage);
                return (bool)objHandler.GetType().GetMethod("UnSub").Invoke(objHandler, new object[] { Router, pMessage });
            } catch (Exception ex) {
                Log.Error(ex);
                return false;
            }
        }

        public bool Notify(RequestMessage pMessage) {
            try {
                var objHandler = MessagingFactory.Provider.GetSubscriptionHandler(pMessage);
                return (bool)objHandler.GetType().GetMethod("Notify").Invoke(objHandler, new object[] { Router, pMessage });
            } catch (Exception ex) {
                Log.Error(ex);
                return false;
            }
        }
    }
}