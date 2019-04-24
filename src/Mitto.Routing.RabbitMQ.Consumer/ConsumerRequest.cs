using System;
using System.Threading.Tasks;
using Mitto.IMessaging;
using Mitto.Logging;

namespace Mitto.Routing.RabbitMQ.Consumer {

    /// <summary>
    /// ToDo: KeepAlive
    /// </summary>
    internal class ConsumerRequest : IMessaging.IRequest {
        private readonly ILog Log = LoggingFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string ID => Request.RequestID;

        public IRouting.MessageStatus Status => throw new NotImplementedException();

        public IRequestMessage Message => throw new NotImplementedException();

        public event EventHandler<IMessaging.IRequest> RequestTimedOut;

        private readonly IRouting.RoutingFrame Request;
        private readonly SenderQueue ProviderQueue;
        private readonly Delegate Action;

        public ConsumerRequest(SenderQueue pProviderQueue, IRouting.RoutingFrame pRequest, Action<IResponseMessage> pCallback) {
            ProviderQueue = pProviderQueue;
            Request = pRequest;
            Action = pCallback;
        }

        /// <summary>
        /// ToDo: do not create a new Router on very send
        /// </summary>
        public void Transmit() {
            Log.Debug($"Sending request with ID {ID}");
            new ConsumerRouter(ID, ProviderQueue, Request).Start();
        }

        public void SetResponse(IResponseMessage pMessage) {
            /*var objRoutingFrame = new RoutingFrame(
                pFrame.FrameType,
                pFrame.MessageType,
                pFrame.RequestID,
                ProviderQueue.QueueName,
                Consumer.ID,
                pFrame.Data
            );
            ProviderQueue.Transmit(objRoutingFrame);*/
            Task.Run(() => {
                Action.DynamicInvoke(pMessage);
            });
        }
    }
}