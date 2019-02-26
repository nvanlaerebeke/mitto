using Mitto.IMessaging;

namespace Mitto.Messaging.Tests.TestData.Response {
	public class ResponseTestMessage : ResponseMessage {
		public ResponseTestMessage() { }
		public ResponseTestMessage(IMessage pMessage, ResponseCode pStatus) : base(pMessage, pStatus) { }
	}
}