namespace Messaging.Base {
	public class MessageClient {
		public string ClientID { get; private set; }
		public IQueue.IQueue Queue { get; private set; }

		public MessageClient(string clientID, IQueue.IQueue pQueue) {
			ClientID = clientID;
			Queue = pQueue;
		}
	}
}
