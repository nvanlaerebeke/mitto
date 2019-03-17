using Mitto.Routing.Response;

namespace Mitto.Routing.Action {
	public interface IControlAction { }
	public interface IControlAction<O> where O : IControlResponse {
		O Start();
	}
}