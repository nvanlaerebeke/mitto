using Mitto.IRouting;
using System;

namespace Mitto.Routing {
	/// <summary>
	/// ToDo: Add KeepAlives that raise the RequestTimedOut
	/// </summary>
	public class FrameRequest : IRequest {
		private readonly IRouter Origin;
		private readonly RoutingFrame Frame;

		public string ID { get { return Frame.RequestID; } }
		public event EventHandler<IRequest> RequestTimedOut;

		public MessageStatus Status { get; private set; } = MessageStatus.UnKnown;

		public FrameRequest(IRouter pRouter, RoutingFrame pRequest) {
			Origin = pRouter;
			Frame = pRequest;
		}

		public void Send() {
			if(
				Frame.FrameType == RoutingFrameType.Control &&
				Frame.MessageType == MessageType.Request
			) {
				ControlFactory.Processor.Process(Origin, Frame);
			}
		}

		public void SetResponse(RoutingFrame pFrame) {
			//no implemented, response sent using the IRouter.Transmit method when being processed
		}
	}
}