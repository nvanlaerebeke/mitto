using Mitto.IMessaging;
using Mitto.IRouting;
using Mitto.Routing.RabbitMQ.Request;
using Mitto.Routing.RabbitMQ.Response;
using Mitto.Routing.Request;
using Mitto.Routing.Response;
using System.Threading;

namespace Mitto.Routing.RabbitMQ.Consumer {

    /// <summary>
    /// ToDo: instead of using a ConsumerRouter per RoutingFrame, use a single Router
    ///       per publisher and then pass the RoutingFrame to said router to be 'routed'
    ///       Router should be responsible for routing ALL messages, so not a single message
    /// </summary>
    internal class ConsumerRouter : IRouter {
        public string ConnectionID { get; private set; }

        public readonly RoutingFrame Request;
        public readonly SenderQueue Publisher;

        public ConsumerRouter(string pConsumerID, SenderQueue pPublisher, RoutingFrame pFrame) {
            ConnectionID = pConsumerID;
            Publisher = pPublisher;
            Request = pFrame;
        }

        public void Start() {
            if (Request.FrameType != RoutingFrameType.Control) {
                ControlFactory.Processor.Request(new ControlRequest<CanStartActionResponse>(
                    this,
                    new CanStartActionRequest(Request.RequestID),
                    (r) => {
                        if (r.CanStart) {
                            MessagingFactory.Processor.Process(this, Request.Data);
                        }
                    }
                ));
            } else {
                if (Request.MessageType == MessageType.Response) {
                }
            }
        }

        public void Close() {
        }

        public void Receive(byte[] pData) {
            //nothing to do, not applicable for RabbitMQ consumer
        }

        /// <summary>
        /// Sends data back to the Publisher this request originated from
        /// ToDo: use RoutingFrame instead of byte[]
        /// </summary>
        /// <param name="pData"></param>
        public void Transmit(byte[] pData) {
            var oldFrame = new RoutingFrame(pData);
            var objFrame = new RoutingFrame(
                oldFrame.FrameType,
                oldFrame.MessageType,
                Request.RequestID,
                Consumer.ID,
                Request.DestinationID,
                oldFrame.Data
            );
            Publisher.Transmit(objFrame);
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