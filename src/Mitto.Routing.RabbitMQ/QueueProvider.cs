using System.Collections.Concurrent;

namespace Mitto.Routing.RabbitMQ {
	public class QueueProvider {
		private ConcurrentDictionary<string, SenderQueue> SenderQueues = new ConcurrentDictionary<string, SenderQueue>();
		private ConcurrentDictionary<string, ReaderQueue> ReaderQueues = new ConcurrentDictionary<string, ReaderQueue>();
		
		public QueueProvider(RabbitMQParams pParams) { }
				
		public ReaderQueue GetReaderQueue(string pQueueID) {
			if(ReaderQueues.ContainsKey(pQueueID)) {
				if(ReaderQueues.TryGetValue(pQueueID, out ReaderQueue objQueue)) {
					return objQueue;
				}
			}
			//Not found or error, create, add and return
			var obj = new ReaderQueue(pQueueID);
			if(!ReaderQueues.TryAdd(pQueueID, obj)) {
				//ToDo: Logging
			}
			return obj;
		}

		public SenderQueue GetSenderQueue(string pQueueID) {
			if (SenderQueues.ContainsKey(pQueueID)) {
				if (SenderQueues.TryGetValue(pQueueID, out SenderQueue objQueue)) {
					return objQueue;
				}
			}
			//Not found or error, create, add and return
			var obj = new SenderQueue(pQueueID);
			if (!SenderQueues.TryAdd(pQueueID, obj)) {
				//ToDo: Logging
			}
			return obj;
		}
	}
}