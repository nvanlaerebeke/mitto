using Mitto.IMessaging;

namespace Mitto.Messaging {
	internal interface IActionManager {
		void RunAction(IClient pClient, IMessage pMessage, IAction pAction);
		MessageStatusType GetStatus(string pRequestID);
	}
}