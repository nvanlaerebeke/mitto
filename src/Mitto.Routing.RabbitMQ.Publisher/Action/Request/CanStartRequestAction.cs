using Mitto.IRouting;
using Mitto.Routing.Action;
using Mitto.Routing.RabbitMQ.Request;
using Mitto.Routing.RabbitMQ.Response;

namespace Mitto.Routing.RabbitMQ.Publisher.Action.Request {

    internal class CanStartActionRequestAction : BaseControlAction<CanStartActionRequest, CanStartActionResponse> {

        public CanStartActionRequestAction(IRouter pConnection, CanStartActionRequest pRequest) : base(pConnection, pRequest) {
        }

        public override CanStartActionResponse Start() {
            return new CanStartActionResponse(Request, RouterProvider.HasRequest(Request.RequestID));
        }
    }
}