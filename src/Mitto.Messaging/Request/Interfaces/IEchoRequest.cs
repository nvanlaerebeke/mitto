using Mitto.IMessaging;

namespace Mitto.Messaging.Request {
	public interface IEchoRequest: IRequestMessage {
		string Message { get; set; }
	}
}