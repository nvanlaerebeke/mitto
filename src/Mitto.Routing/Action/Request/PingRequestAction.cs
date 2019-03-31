using Mitto.IRouting;
using Mitto.Routing.Action;
using Mitto.Routing.Request;
using Mitto.Routing.Response;

namespace Mitto.Routing.Action.Request {

	internal class PingRequestAction : BaseControlAction<PingRequest, PongResponse> {

		public PingRequestAction(IRouter pConnection, PingRequest pRequest) : base(pConnection, pRequest) { }

		public override PongResponse Start() {
			return new PongResponse(Request);
		}
	}
}