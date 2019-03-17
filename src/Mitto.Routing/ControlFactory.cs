namespace Mitto.Routing {
	public class ControlFactory {
		public static ControlProcessor Processor { get; } = new ControlProcessor();
		public static ControlProvider Provider { get; } = new ControlProvider();
	}
}
