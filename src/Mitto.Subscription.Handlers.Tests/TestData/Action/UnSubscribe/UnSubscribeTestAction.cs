using Mitto.IMessaging;
using Mitto.Messaging.Action;
using Mitto.Messaging.Response;
using Mitto.Subscription.Messaging.Tests.TestData.UnSubscribe;

namespace Mitto.Subscription.Messaging.Tests.TestData.Action.UnSubscribe {

    public class UnSubscribeTestAction : RequestAction<UnSubscribeTestMessage, ACKResponse> {

        public UnSubscribeTestAction(IClient pClient, UnSubscribeTestMessage pRequest) : base(pClient, pRequest) {
        }

        public override ACKResponse Start() {
            return new ACKResponse(Request);
        }
    }
}