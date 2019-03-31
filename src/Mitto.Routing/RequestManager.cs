using Mitto.IRouting;
using System.Collections.Concurrent;
using Mitto.ILogging;
using System;

namespace Mitto.Routing {
	public class RequestManager {
		private ILog Log {
			get {
				return LogFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
			}
		}

		private ConcurrentDictionary<string, IRequest> Requests = new ConcurrentDictionary<string, IRequest>();

		public void Send(IRequest pRequest) {
			pRequest.RequestTimedOut += Request_RequestTimedOut;
			if(!Requests.TryAdd(pRequest.ID, pRequest)) {
				Log.Error($"Unable to add request {pRequest.ID} to RequestManager list, response will be dropped");
			}
			pRequest.Send();
		}

		public void Receive(IRouter pOrigin, RoutingFrame pFrame) {
			//Is a response to an active request?

			if (pFrame.FrameType == RoutingFrameType.Control && pFrame.MessageType == MessageType.Request) {
				Console.WriteLine($"Processing request with ID {pFrame.RequestID}");
				var objRequest = new FrameRequest(pOrigin, pFrame);
				objRequest.RequestTimedOut += Request_RequestTimedOut;
				Requests.TryAdd(objRequest.ID, objRequest);
				objRequest.Send();
			} else if(pFrame.FrameType == RoutingFrameType.Control && pFrame.MessageType == MessageType.Response) {
				if(Requests.ContainsKey(pFrame.RequestID)) {
					Requests[pFrame.RequestID].SetResponse(pFrame);
					if(!Requests.TryRemove(pFrame.RequestID, out _)) {
						//ToDo: Error Logging
					}
				}
			} else if (pFrame.MessageType == MessageType.Response) {
				if (Requests.ContainsKey(pFrame.RequestID)) {
					Requests[pFrame.RequestID].SetResponse(pFrame);
					if (!Requests.TryRemove(pFrame.RequestID, out _)) {
						Log.Error($"Unable to remove request {pFrame.RequestID} from list, will time out on keepalive timeout");
					}
				} else {
					Log.Error($"Dropping response, no request found for {pFrame.RequestID}");
				}
				return;
			} else {
				Log.Error($"Request {pFrame.RequestID} not found, no one listening?, ignoring...");
			}
		}

		public bool ContainsRequest(string pRequestID) {
			return Requests.ContainsKey(pRequestID);
		}

		public MessageStatus GetStatus(string pRequestID) {
			Console.WriteLine($"Getting Status For: {pRequestID}");
			if (Requests.ContainsKey(pRequestID)) {
				return Requests[pRequestID].Status;
			} else {
				return MessageStatus.UnKnown;
			}
		}

		private void Request_RequestTimedOut(object sender, IRequest e) {
			e.RequestTimedOut -= Request_RequestTimedOut;
			if (!Requests.TryRemove(e.ID, out _)) {
				//ToDo: error logging
			}
		}
	}
}