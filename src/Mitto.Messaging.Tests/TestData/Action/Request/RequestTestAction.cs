using Mitto.IMessaging;
using Mitto.Messaging.Action;
using Mitto.Messaging.Tests.TestData.Request;

namespace Mitto.Messaging.Tests.TestData.Action.Request {
	public class RequestTestAction : RequestAction<RequestTestMessage> {
		public RequestTestAction(IClient pClient, RequestTestMessage pMessage) : base(pClient, pMessage) { }

		public override IResponseMessage Start() {
			return new Response.ResponseTestMessage();
		}
	}
}
