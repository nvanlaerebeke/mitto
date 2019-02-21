using Mitto.IMessaging;

namespace Mitto.Messaging {
	interface IMessageRequest {
		void SetResponse(IResponseMessage pResponse);
	}
}
