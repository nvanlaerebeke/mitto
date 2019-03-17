using Mitto.IRouting;
using Mitto.Routing.Action;
using Mitto.Routing.Request;
using Mitto.Routing.Response;

namespace Mitto.Routing.RabbitMQ.Publisher.Action.Request {
	public class GetMessageStatusRequestAction : BaseControlAction<GetMessageStatusRequest, GetMessageStatusResponse> {
		public GetMessageStatusRequestAction(IRouter pConnection, GetMessageStatusRequest pRequest) : base(pConnection, pRequest) { }

		public override GetMessageStatusResponse Start() {
			return new GetMessageStatusResponse(Request, true);
		}
	}
}