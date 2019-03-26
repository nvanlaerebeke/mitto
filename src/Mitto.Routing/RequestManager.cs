using Mitto.IRouting;
using System.Collections.Concurrent;
using Mitto.ILogging;

namespace Mitto.Routing {
	/// <summary>
	/// ToDo: Add RequestID to RoutingFrame so Mitto.Routing can be decoupled entirely from IMessaging
	/// Removing the reference to IMessage
	/// </summary>
	public class RequestManager {
		private ILog Log {
			get {
				return LogFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
			}
		}
		private ConcurrentDictionary<string, IRequest> Requests = new ConcurrentDictionary<string, IRequest>();

		public void Send(IRequest pRequest) {
			Requests.TryAdd(pRequest.ID, pRequest);
			pRequest.Send();
		}

		/// <summary>
		/// Sends the Frame to the target
		/// </summary>
		/// <param name="pFrame"></param>
		/*public void Send(IRouter pRouter, RoutingFrame pFrame) {
			if (Requests.ContainsKey(pFrame.RequestID)) {
				Requests[pFrame.RequestID].Set(pFrame);
				if (!Requests.TryRemove(pFrame.RequestID, out _)) {
					Log.Error($"Unable to remove request for list, will time out due to keepalive, can be ignored");
				} 
			} else {
				//Log.Error("");
			}
		}*/

		public void Receive(RoutingFrame pFrame) {
			if (Requests.ContainsKey(pFrame.RequestID)) {
				Requests[pFrame.RequestID].SetResponse(pFrame);
				if (!Requests.TryRemove(pFrame.RequestID, out _)) {
					Log.Error($"Unable to remove request {pFrame.RequestID} from list, will time out on keepalive timeout");
				}
				return;
			} else {
				Log.Error($"Request {pFrame.RequestID} not found, no one listening?, ignoring...");
			}
		}

		public MessageStatus GetStatus(string pRequestID) {
			if (Requests.ContainsKey(pRequestID)) {
				return Requests[pRequestID].Status;
			} else {
				return MessageStatus.UnKnown;
			}
		}
	}
}