using Mitto.IConnection;

namespace Mitto.IRouting {
	public interface IRouterProvider {
		IRouter Create(IClientConnection pConnection);
	}
}