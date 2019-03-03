using Mitto.IMessaging;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("Mitto.Messaging.Json.Tests")]
namespace Mitto.Messaging {
	internal interface IRequestManager {
		void Request<T>(IRequest pRequest) where T : IResponseMessage;
		void SetResponse(IResponseMessage pMessage);
		MessageStatusType GetStatus(string pRequestID);
	}
}