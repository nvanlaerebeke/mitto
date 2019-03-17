using Mitto.IRouting;

namespace Mitto.Routing.Request {
	public abstract class ControlRequest : ControlMessage, IControlRequest {
		public string ID { get; protected set; } = System.Guid.NewGuid().ToString();
	}
}