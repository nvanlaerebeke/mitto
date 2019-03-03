using Mitto.IMessaging;
using Mitto.Messaging.Action;

namespace Mitto.Messaging.Tests.TestData.Action.UnSubscribe {
	public class UnSubscribeTestAction : RequestAction<TestData.UnSubscribe.UnSubscribeTestMessage> {
		public UnSubscribeTestAction(IClient pClient, TestData.UnSubscribe.UnSubscribeTestMessage pRequest) : base(pClient, pRequest) { }

		public override IResponseMessage Start() {
			return new Messaging.Response.ACK(Request, ResponseCode.Success);
		}
	}
}
