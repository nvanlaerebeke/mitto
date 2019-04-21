using Mitto.IMessaging;
using Mitto.IRouting;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Mitto.Messaging {

    internal interface IActionManager {

        void RunAction(IClient pClient, IRequestMessage pMessage, IAction pAction);

        MessageStatus GetStatus(string pRequestID);
    }
}