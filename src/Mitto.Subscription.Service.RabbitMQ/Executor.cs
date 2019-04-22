using ILogging;
using Logging;
using Mitto.IMessaging;
using Mitto.IRouting;
using Mitto.Messaging;
using Mitto.Messaging.Response;
using Mitto.Subscription.Messaging;
using System;

namespace Mitto.Subscription.Service.RabbitMQ {

    /// <summary>
    /// ToDo: Rethink the design of this - this class does what ActionManager does with a Request
    /// </summary>
    internal class Executor {
        private ILog Log { get { return LogFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType); } }
        private readonly PublisherRouter Router;

        public Executor(PublisherRouter pRouter) {
            Router = pRouter;
        }

        public bool Sub(SubMessage pMessage) {
            return Handle("Sub", pMessage);
        }

        public bool UnSub(UnSubMessage pMessage) {
            return Handle("UnSub", pMessage);
        }

        public bool Notify(RequestMessage pMessage) {
            return Handle("Notify", pMessage);
        }

        private bool Handle(string pType, IRequestMessage pMessage) {
            try {
                var objHandler = MessagingFactory.Provider.GetSubscriptionHandler(pMessage);
                var blnResult = (bool)objHandler.GetType().GetMethod(pType).Invoke(objHandler, new object[] { Router, pMessage });
                var objResponse = new ACKResponse(pMessage, new ResponseStatus((blnResult) ? ResponseState.Success : ResponseState.Error));
                var arrBytes = MessagingFactory.Converter.GetByteArray(objResponse);
                var objRoutingFrame = new RoutingFrame(
                    RoutingFrameType.Messaging,
                    objResponse.Type,
                    objResponse.ID,
                    Router.SourceID,
                    Router.DestinationID,
                    arrBytes
                );
                Router.ConsumerQueue.Transmit(objRoutingFrame);
                return blnResult;
            } catch (Exception ex) {
                Log.Error(ex);
                return false;
            }
        }
    }
}