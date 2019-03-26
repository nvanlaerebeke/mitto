using Mitto.IRouting;
using Mitto.Routing.RabbitMQ.Publisher.Message.Request;
using Mitto.Routing.Response;

namespace Mitto.Routing.RabbitMQ.Publisher.Message.Response {

	public class CanStartActionResponse : ControlResponse {
		public bool CanStart { get; set; }

		public CanStartActionResponse(CanStartActionRequest pRequest, bool pCanStart) : base(pRequest.ID) {
			CanStart = pCanStart;
		}

		public CanStartActionResponse(ControlFrame pFrame) : base(pFrame.RequestID) {
			CanStart = (pFrame.Data[0] == 1);
		}

		public override ControlFrame GetFrame() {
			return GetFrame(new byte[] { (byte)(CanStart ? 1 : 0) });
		}
	}
}