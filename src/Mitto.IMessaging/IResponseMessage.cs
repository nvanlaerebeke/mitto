namespace Mitto.IMessaging {
	public interface IResponseMessage : IMessage {
		ResponseStatus Status { get; }
		IMessage Request { get;  }
    }
}