using Mitto.IMessaging;
using Mitto.IRouting;

namespace Mitto.Messaging.Tests.TestData.Response {
	public class ResponseTestMessage : ResponseMessage {
		public ResponseTestMessage() { }
		public ResponseTestMessage(IRequestMessage pMessage, ResponseStatus pStatus) : base(pMessage, pStatus) { }
	}
}