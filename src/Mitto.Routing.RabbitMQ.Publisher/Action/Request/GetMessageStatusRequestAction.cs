using Mitto.IRouting;
using Mitto.Routing.Action;
using Mitto.Routing.Request;
using Mitto.Routing.Response;

namespace Mitto.Routing.RabbitMQ.Publisher.Action.Request {

	internal class GetMessageStatusRequestAction : BaseControlAction<GetMessageStatusRequest, GetMessageStatusResponse> {
		private readonly Router Router;

		public GetMessageStatusRequestAction(IRouter pConnection, GetMessageStatusRequest pRequest) : base(pConnection, pRequest) {
			Router = pConnection as Router;
		}

		public override GetMessageStatusResponse Start() {
			if (Router != null) {
				return new GetMessageStatusResponse(
					Request,
					(Router.RequestManager.GetStatus(Request.RequestID) != MessageStatus.UnKnown)
				);
			}
			return new GetMessageStatusResponse(Request, false);
		}
	}
}