using Mitto.IMessaging;
using Mitto.Messaging.Action;
using Mitto.Messaging.Response;
using Mitto.Subscription.Messaging.Tests.TestData.Subscribe;

namespace Mitto.Subscription.Messaging.Tests.TestData.Action.Subscribe {

    public class SubscribeTestAction : RequestAction<SubscribeTestMessage, ACKResponse> {

        public SubscribeTestAction(IClient pClient, SubscribeTestMessage pRequest) : base(pClient, pRequest) {
        }

        public override ACKResponse Start() {
            return new ACKResponse(Request);
        }
    }
}