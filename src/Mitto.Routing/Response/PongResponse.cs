using Mitto.IRouting;
using Mitto.Routing.Request;

namespace Mitto.Routing.Response {
	public class PongResponse : ControlResponse {

		public PongResponse(PingRequest pRequest) : base(pRequest.ID) { }

		public PongResponse(ControlFrame pFrame) : base(pFrame.RequestID) { }

		public override ControlFrame GetFrame() {
			return GetFrame(new byte[] { });
		}
	}
}