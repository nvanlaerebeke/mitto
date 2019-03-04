namespace Mitto.IMessaging {
	public interface IResponseMessage : IMessage {
		ResponseStatus Status { get; }
    }
}