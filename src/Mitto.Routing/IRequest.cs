
using Mitto.IRouting;

namespace Mitto.Routing {
	public interface IRequest {
		string ID { get; }
		MessageStatus Status { get; }
		void Send();
		void SetResponse(RoutingFrame pFrame);
	}
}