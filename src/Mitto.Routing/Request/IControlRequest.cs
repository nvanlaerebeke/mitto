namespace Mitto.Routing.Request {
	public interface IControlRequest : IControlMessage {
		string ID { get; }
	}
}