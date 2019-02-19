using Mitto.IMessaging;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Mitto.Messaging {
	internal interface IRequestManager {
		void Request<R>(MessageClient pClient, IMessage pMessage, Action<R> pAction) where R : IResponseMessage;
		void SetResponse(IResponseMessage pMessage);
		void Process(IQueue.IQueue pClient, IMessage pMessage);
	}
}
