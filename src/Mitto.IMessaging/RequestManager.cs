using Mitto.IRouting;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("Mitto.Messaging.Json.Tests")]
namespace Mitto.IMessaging {
	/// <summary>
	/// ToDo: Move back to Mitto.Messaging, IMessaging is for interfaces only
	/// Starts a request and runs the given action when a response is given
	/// For message types where we do not care about waiting for a response we return right away
	///</summary>
	public class RequestManager : IRequestManager {
		private ConcurrentDictionary<string, IRequest> Requests = new ConcurrentDictionary<string, IRequest>();

		public void Request<T>(IRequest pRequest) where T : IResponseMessage {
			if (Requests.TryAdd(pRequest.Message.ID, pRequest)) {
				pRequest.RequestTimedOut += RequestTimedOut;
				pRequest.Transmit();
			}
		}

		public void SetResponse(IResponseMessage pMessage) {
			if (
				Requests.ContainsKey(pMessage.ID) &&
				Requests.TryRemove(pMessage.ID, out IRequest objRequest)
			) {
				objRequest.RequestTimedOut -= RequestTimedOut;
				objRequest.SetResponse(pMessage);
			}
		}

		public MessageStatus GetStatus(string pRequestID) {
			if(Requests.ContainsKey(pRequestID)) {
				return MessageStatus.Queued;
			}
			return MessageStatus.UnKnown;
		}

		private void RequestTimedOut(object sender, IRequest e) {
			SetResponse(MessagingFactory.Provider.GetResponseMessage(e.Message, new ResponseStatus(ResponseState.TimeOut)));
		}
	}
}