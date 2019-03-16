using Mitto.IMessaging;

namespace Mitto.Messaging.Control {
	public interface IMessageStatusRequest : IRequestMessage {
		string RequestID { get; set; }
	}
}