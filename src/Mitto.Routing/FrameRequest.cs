using ILogging;
using Logging;
using Mitto.IRouting;
using Mitto.Utilities;
using System;

namespace Mitto.Routing {

    /// <summary>
    /// ToDo: Add KeepAlives that raise the RequestTimedOut
    /// </summary>
    public class FrameRequest : IRequest {
        private readonly ILog Log = LogFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IRouter Origin;
        private readonly RoutingFrame Frame;

        public string ID { get { return Frame.RequestID; } }

        public event EventHandler<IRequest> RequestTimedOut;

        public MessageStatus Status { get; private set; } = MessageStatus.UnKnown;

        private readonly IKeepAliveMonitor KeepAliveMonitor;

        public FrameRequest(IRouter pRouter, RoutingFrame pRequest) {
            Origin = pRouter;
            Frame = pRequest;

            KeepAliveMonitor = new KeepAliveMonitor(15);
        }

        public void Send() {
            if (
                Frame.FrameType == RoutingFrameType.Control &&
                Frame.MessageType == MessageType.Request
            ) {
                ControlFactory.Processor.Process(Origin, Frame);
            }

            KeepAliveMonitor.Start();
            KeepAliveMonitor.TimeOut += KeepAliveMonitor_TimeOut;
        }

        private void KeepAliveMonitor_TimeOut(object sender, EventArgs e) {
            Log.Info($"Frame {Frame.RequestID} timed out");

            KeepAliveMonitor.TimeOut -= KeepAliveMonitor_TimeOut;
            KeepAliveMonitor.Stop();

            RequestTimedOut?.Invoke(this, this);
        }

        public void SetResponse(RoutingFrame pFrame) {
            //no implemented, response sent using the IRouter.Transmit method when being processed
        }
    }
}