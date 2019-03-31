namespace Mitto.Routing {
	/// <summary>
	/// ToDo: move to Mitto.Routing.Control
	/// </summary>
	public class ControlFactory {
		public static ControlProcessor Processor { get; } = new ControlProcessor();
		public static ControlProvider Provider { get; } = new ControlProvider();
	}
}
