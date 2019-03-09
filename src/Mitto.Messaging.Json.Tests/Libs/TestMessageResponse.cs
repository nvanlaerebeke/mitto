using System;
using Mitto.IMessaging;

namespace Mitto.Messaging.Json.Tests.Libs {
	public class TestMessageResponse : IResponseMessage {
		public TestMessageResponse(IRequestMessage pRequest, ResponseStatus pStatus) {
			Request = pRequest;
			Status = pStatus;
			StartTime = pRequest.StartTime;
		}
		public string ID => "MyID";

		public string Name => "TestMessageResponse";

		public MessageType Type => MessageType.Response;

		public ResponseStatus Status { get; set; }

		public IMessage Request { get; set; }

		public DateTime StartTime { get; private set; }

		public DateTime EndTime => DateTime.Now;

		public byte GetCode() { return 0x66;  }
	}
}
