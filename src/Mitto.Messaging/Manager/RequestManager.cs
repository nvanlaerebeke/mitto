using Mitto.IMessaging;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("Mitto.Messaging.Json.Tests")]
namespace Mitto.Messaging {
	/// <summary>
	/// ToDo:
	/// Add the following methods do do requests:
	/// - based on a callback method, does not block, IResponseMessage Client.Request<IResponseMessage>(IRequestMessage, Action<IResponseMessage>)
	/// - synchonious, blocks thread, IResponseMessage Client.Request<IResponseMessage>(IRequestMessage)
	/// - async, Task<IResponseMessage> Client.RequesAsync<IResponseMessage>(IRequestMessage)
	///
	/// Starts a request and runs the given action when a response is given
	/// For message types where we do not care about waiting for a response we return right away
	///</summary>
	internal class RequestManager : IRequestManager {
        ConcurrentDictionary<string, IRequest> Requests = new ConcurrentDictionary<string, IRequest>();

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

		public MessageStatusType GetStatus(string pRequestID) {
			if(Requests.ContainsKey(pRequestID)) {
				return MessageStatusType.Queued;
			}
			return MessageStatusType.UnKnown;
		}

		private void RequestTimedOut(object sender, IRequest e) {
			SetResponse(MessagingFactory.Provider.GetResponseMessage(e.Message, new ResponseStatus(ResponseState.TimeOut)));
		}
	}
}