using Mitto.IMessaging;

namespace Mitto.Messaging.Json.Tests.Libs {
	public class TestMessageResponse : IResponseMessage {
		public TestMessageResponse(IMessage pRequest, ResponseStatus pStatus) {
			Request = pRequest;
			Status = pStatus;
		}
		public string ID => "MyID";

		public string Name => "TestMessageResponse";

		public MessageType Type => MessageType.Response;

		public ResponseStatus Status { get; set; }

		public IMessage Request { get; set; }

		public byte GetCode() { return 0x66;  }
	}
}
