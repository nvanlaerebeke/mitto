using Mitto.IMessaging;
using Mitto.IRouting;

namespace Mitto.Messaging {
	internal interface IActionManager {
		void RunAction(IClient pClient, IRequestMessage pMessage, IAction pAction);
		MessageStatus GetStatus(string pRequestID);
	}
}