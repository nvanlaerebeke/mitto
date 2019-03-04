using Mitto.IMessaging;

namespace Mitto.Messaging.Request {
	public interface IMessageStatusRequest : IRequestMessage {
		string RequestID { get; set; }
	}
}