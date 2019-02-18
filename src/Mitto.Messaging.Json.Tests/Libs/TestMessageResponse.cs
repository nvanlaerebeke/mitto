using Mitto.IMessaging;

namespace Mitto.Messaging.Json.Tests.Libs {
	public class TestMessageResponse : IResponseMessage {
		public TestMessageResponse(IMessage pRequest, ResponseCode pResponse) {
			Request = pRequest;
			Status = pResponse;
		}
		public string ID => "MyID";

		public string Name => "TestMessageResponse";

		public MessageType Type => MessageType.Response;

		public ResponseCode Status { get; set; }

		public IMessage Request { get; set; }

		public byte GetCode() { return 0x66;  }
	}
}
