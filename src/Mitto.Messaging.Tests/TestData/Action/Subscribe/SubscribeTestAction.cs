using Mitto.IMessaging;
using Mitto.Messaging.Action;

namespace Mitto.Messaging.Tests.TestData.Action.Subscribe {
	public class SubscribeTestAction : RequestAction<TestData.Subscribe.SubscribeTestMessage> {
		public SubscribeTestAction(IClient pClient, TestData.Subscribe.SubscribeTestMessage pRequest) : base(pClient, pRequest) { }

		public override IResponseMessage Start() {
			return new Messaging.Response.ACK(Request, ResponseCode.Success);
		}
	}
}
