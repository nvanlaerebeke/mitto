namespace Mitto.Messaging.Request {
	public class ReceiveBinaryDataRequest : RequestMessage {
		public byte[] Data { get; set; }

		public ReceiveBinaryDataRequest() { }
		public ReceiveBinaryDataRequest(byte[] pData) {
			Data = pData;
		}
	}
}