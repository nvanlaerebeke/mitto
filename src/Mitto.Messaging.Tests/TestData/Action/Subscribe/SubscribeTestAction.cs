using Mitto.IMessaging;
using Mitto.Messaging.Action;
using Mitto.Messaging.Response;
using Mitto.Messaging.Tests.TestData.Subscribe;

namespace Mitto.Messaging.Tests.TestData.Action.Subscribe {
	public class SubscribeTestAction : RequestAction<SubscribeTestMessage, ACKResponse> {
		public SubscribeTestAction(IClient pClient, SubscribeTestMessage pRequest) : base(pClient, pRequest) { }

		public override IResponseMessage Start() {
			return new ACKResponse(Request, ResponseCode.Success);
		}
	}
}
