namespace Mitto.IMessaging {
	public interface IResponseMessage : IMessage {
		ResponseCode Status { get; }
		IMessage Request { get;  }
        void SetResponse(IResponseMessage pResponse)

    }
}
