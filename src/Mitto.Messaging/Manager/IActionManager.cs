using Mitto.IMessaging;

namespace Mitto.Messaging {
	internal interface IActionManager {
		void RunAction(IClient pClient, IRequestMessage pMessage, IAction pAction);
		MessageStatusType GetStatus(string pRequestID);
	}
}