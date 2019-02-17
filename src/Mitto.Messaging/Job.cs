namespace Mitto.Messaging {
	public class Job {
		public MessageClient Client;
		public byte[] Data { get; private set; }

		public Job(MessageClient pClient, byte[] pData) {
			Client = pClient;
			Data = pData;
		}
	}
}