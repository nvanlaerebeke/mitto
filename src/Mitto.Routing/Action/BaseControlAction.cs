using Mitto.IRouting;
using Mitto.Routing.Request;
using Mitto.Routing.Response;

namespace Mitto.Routing.Action {
	public abstract class BaseControlAction<I, O> : IControlAction<O>, IControlAction
		where I: IControlRequest
		where O: IControlResponse
	{
		public readonly IRouter Connection;
		public readonly I Request;

		public BaseControlAction(IRouter pConnection, I pRequest) {
			Connection = pConnection;
			Request = pRequest;
		}

		public abstract O Start();
	}
}
