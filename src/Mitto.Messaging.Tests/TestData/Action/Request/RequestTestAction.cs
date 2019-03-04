using Mitto.IMessaging;
using Mitto.Messaging.Action;
using Mitto.Messaging.Tests.TestData.Request;
using Mitto.Messaging.Tests.TestData.Response;

namespace Mitto.Messaging.Tests.TestData.Action.Request {
	public class RequestTestAction : RequestAction<RequestTestMessage, ResponseTestMessage> {
		public RequestTestAction(IClient pClient, RequestTestMessage pMessage) : base(pClient, pMessage) { }

		public override ResponseTestMessage Start() {
			return new ResponseTestMessage();
		}
	}
}
