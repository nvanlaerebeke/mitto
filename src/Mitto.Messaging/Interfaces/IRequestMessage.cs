using Mitto.IMessaging;

namespace Mitto.Messaging {
	interface IRequestMessage {
		void SetResponse(IResponseMessage pMessage);
	}
}
