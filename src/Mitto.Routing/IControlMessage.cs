using Mitto.IRouting;

namespace Mitto.Routing {

	public interface IControlMessage {
		string ID { get; }

		ControlFrame GetFrame();
	}
}