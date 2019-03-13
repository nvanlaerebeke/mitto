using System.Collections.Concurrent;
using Mitto.IMessaging;

namespace Mitto.Routing.RabbitMQ.Publisher {
	/// <summary>
	/// ToDo: add a keepalive for the Queues
	///       kept alive due to not having to create new 
	///       threads and SenderQueue objects as those are 
	///       more expensive to create
	///       
	/// ToDo: make the Queues static/shared between all IClientConnections
	///       request will be spread to all Consumers so it's a good idea 
	///       to not always create a new SenderQueue for each IClientConnection 
	///       but just share them.
	///       Example: 
	///         - without sharing: with 500 clients and 5 workers there are 2500 SenderQueues
	///         - with sharing: with 500 clients and 5 workers, there are 5 SenderQueues
	/// </summary>
	internal class RequestManager {
		private ConcurrentDictionary<string, string> Requests = new ConcurrentDictionary<string, string>();
		private ConcurrentDictionary<string, SenderQueue> Queues = new ConcurrentDictionary<string, SenderQueue>();

		public void AddRequest(Frame pFrame) {
			if (!Queues.ContainsKey(pFrame.QueueID)) {
				if (!Queues.TryAdd(pFrame.QueueID, new SenderQueue(pFrame.QueueID))) {
					//ToDo: error handling
				}
			}
			if(!Requests.TryAdd(pFrame.MessageID, pFrame.QueueID)) {
				//ToDo: error handling
			}
		}

		public void SetResponse(Frame pFrame) {
			if (
				Requests.ContainsKey(pFrame.MessageID) &&
				Requests.TryRemove(pFrame.MessageID, out string strQueueID)
			) {
				//Queues[strQueueID].Transmit(pFrame);
			} else {
				//ToDo: Error handling
			}
		}
	}
}