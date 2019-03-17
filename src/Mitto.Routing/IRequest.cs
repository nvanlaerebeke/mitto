
using Mitto.IRouting;

namespace Mitto.Routing {
	public interface IRequest {
		string ID { get; }
		void Send();
		void Set(ControlFrame pFrame);
	}
}