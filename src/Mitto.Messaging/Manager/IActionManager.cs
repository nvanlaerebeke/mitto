using Mitto.IMessaging;
using Mitto.IRouting;
using System.Runtime.CompilerServices;

namespace Mitto.Messaging {

    public interface IActionManager {

        void RunAction(IClient pClient, IRequestMessage pMessage, IAction pAction);

        MessageStatus GetStatus(string pRequestID);
    }
}