namespace IQueue {
	public class Message {
		public string ClientID { get; private set; }
		public byte[] Data { get; private set; }
		public Message(string pClientID, byte[] pData) {
			ClientID = pClientID;
			Data = pData;
		}
	}
}