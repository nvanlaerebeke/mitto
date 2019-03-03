using Mitto.IMessaging;

namespace Mitto.Messaging.Tests.TestData.Response {
	public class ResponseTestMessage : ResponseMessage {
		public ResponseTestMessage() { }
		public ResponseTestMessage(IRequestMessage pMessage, ResponseCode pStatus) : base(pMessage, pStatus) { }
	}
}