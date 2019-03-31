using System.Collections.Concurrent;

namespace Mitto.Routing.RabbitMQ {
	public class QueueProvider {
		private ConcurrentDictionary<string, SenderQueue> SenderQueues = new ConcurrentDictionary<string, SenderQueue>();
		private ConcurrentDictionary<string, ReaderQueue> ReaderQueues = new ConcurrentDictionary<string, ReaderQueue>();
		
		public QueueProvider(RabbitMQParams pParams) { }
				
		public ReaderQueue GetReaderQueue(QueueType pType, string pQueueID, bool pShared) {
			if(ReaderQueues.ContainsKey(pQueueID)) {
				if(ReaderQueues.TryGetValue(pQueueID, out ReaderQueue objQueue)) {
					return objQueue;
				}
			}
			//Not found or error, create, add and return
			var obj = new ReaderQueue(pType, pQueueID, pShared);
			if(!ReaderQueues.TryAdd(pQueueID, obj)) {
				//ToDo: Logging
			}
			return obj;
		}

		public SenderQueue GetSenderQueue(QueueType pType, string pQueueID, bool pShared) {
			if (SenderQueues.ContainsKey(pQueueID)) {
				if (SenderQueues.TryGetValue(pQueueID, out SenderQueue objQueue)) {
					return objQueue;
				}
			}
			//Not found or error, create, add and return
			var obj = new SenderQueue(pType, pQueueID, pShared);
			if (!SenderQueues.TryAdd(pQueueID, obj)) {
				//ToDo: Logging
			}
			return obj;
		}
	}
}