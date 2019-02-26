using Mitto.IMessaging;

namespace Mitto.Messaging {
	internal interface IActionManager {
		bool IsBusy(string pID);
		void RunAction(IQueue.IQueue pClient, IMessage pMessage, IAction pAction);
	}
}