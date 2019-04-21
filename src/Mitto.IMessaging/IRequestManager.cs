using Mitto.IMessaging;
using Mitto.IRouting;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("Mitto.Messaging.Json.Tests")]
namespace Mitto.IMessaging {
	public interface IRequestManager {
		void Request<T>(IRequest pRequest) where T : IResponseMessage;
		void SetResponse(IResponseMessage pMessage);
		MessageStatus GetStatus(string pRequestID);
	}
}