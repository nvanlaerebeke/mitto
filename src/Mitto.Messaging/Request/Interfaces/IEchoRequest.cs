using Mitto.IMessaging;
using Mitto.IRouting;

namespace Mitto.Messaging.Request {
	public interface IEchoRequest: IRequestMessage {
		string Message { get; set; }
	}
}