using Mitto.IMessaging;
using Mitto.IRouting;

namespace Mitto.Messaging {
	interface IMessageRequest {
		void SetResponse(IResponseMessage pResponse);
	}
}
