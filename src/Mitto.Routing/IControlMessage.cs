using Mitto.IRouting;

namespace Mitto.Routing {
	public interface IControlMessage {
		ControlFrame GetFrame();
	}
}