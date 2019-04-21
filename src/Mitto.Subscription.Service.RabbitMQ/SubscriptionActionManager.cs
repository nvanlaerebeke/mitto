using Mitto.ILogging;
using Mitto.IMessaging;
using Mitto.IRouting;
using Mitto.Messaging;
using Mitto.Messaging.Response;
using Mitto.Subscription.Messaging;
using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Mitto.Subscription.Service.RabbitMQ {
    public class SubscriptionActionManager : IActionManager {
        private ILog Log {
            get { return LogFactory.GetLogger(MethodBase.GetCurrentMethod().DeclaringType); }
        }

        private ConcurrentDictionary<string, IRequestMessage> Actions = new ConcurrentDictionary<string, IRequestMessage>();

        public MessageStatus GetStatus(string pRequestID) {
            return Actions.ContainsKey(pRequestID) ? MessageStatus.Busy : MessageStatus.UnKnown;
        }

        public void RunAction(IClient pClient, IRequestMessage pMessage, IAction pAction) {
            if (Actions.TryAdd(pMessage.ID, pMessage)) {
                var objRouter = pClient.Router as PublisherRouter;
                if (pMessage is SubMessage) {
                    Handle("Sub", objRouter, pMessage);
                } else if (pMessage is UnSubMessage) {
                    Handle("UnSub", objRouter, pMessage);
                } else if (pMessage is RequestMessage) {
                    Handle("Notify", objRouter, pMessage);
                }
                Actions.TryRemove(pMessage.ID, out _);
            }
        }

        private bool Handle(string pType, PublisherRouter pRouter, IRequestMessage pMessage) {
            try {
                var objHandler = MessagingFactory.Provider.GetSubscriptionHandler(pMessage);
                var blnResult = (bool)objHandler.GetType().GetMethod(pType).Invoke(objHandler, new object[] { pRouter, pMessage });
                var objResponse = new ACKResponse(pMessage, new ResponseStatus((blnResult) ? ResponseState.Success : ResponseState.Error));
                var arrBytes = MessagingFactory.Converter.GetByteArray(objResponse);
                var objRoutingFrame = new RoutingFrame(
                    RoutingFrameType.Messaging,
                    objResponse.Type,
                    objResponse.ID,
                    pRouter.SourceID,
                    pRouter.DestinationID,
                    arrBytes
                );
                Log.Debug($"Sending response for request {pMessage.ID}  to {pRouter.ConsumerQueue.QueueName}");
                pRouter.ConsumerQueue.Transmit(objRoutingFrame);
                return blnResult;
            } catch (Exception ex) {
                Log.Error(ex);
            }
            return false;
        }
    }
}