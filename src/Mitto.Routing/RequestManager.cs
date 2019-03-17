using Mitto.IRouting;
using System.Collections.Concurrent;

namespace Mitto.Routing {
	class RequestManager {
		ConcurrentDictionary<string, IRequest> Requests = new ConcurrentDictionary<string, IRequest>();
		internal void Send(IRequest pRequest) {
			Requests.TryAdd(pRequest.ID, pRequest);
			pRequest.Send();
		}

		/// <summary>
		/// Starts an control action when a request arrives, sets the response on 
		/// the request if a response is received
		/// 
		/// ToDo: modify ControlFrame to include a type Request/Respose
		/// Currently if a request isn't found for the given ID it's assumed it's a request
		/// while it's not 100% sure it is
		/// </summary>
		/// <param name="pFrame"></param>
		public void Process(ControlFrame pFrame) {
			if (Requests.ContainsKey(pFrame.RequestID)) {
				if (Requests.TryRemove(pFrame.RequestID, out IRequest objRequest)) {
					objRequest.Set(pFrame);
				} else {
					//ToDo: Logging/error handling
				}
			} else {
				//ToDo: Logging/error handling
			}
		}
	}
}