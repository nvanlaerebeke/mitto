using Mitto.IRouting;

namespace Mitto.Routing {
	/// <summary>
	/// ToDo: move to IRouting
	/// </summary>

	public interface IControlMessage {
		string ID { get; }

		ControlFrame GetFrame();
	}
}