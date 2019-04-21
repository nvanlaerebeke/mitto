using Mitto.IMessaging;
using Mitto.Messaging.Action;
using Mitto.Messaging.Response;
using Mitto.Subscription.Messaging.Handlers;
using Mitto.Subscription.Messaging.Subscribe;

namespace Mitto.Subscription.Messaging.Action.Subscribe {

    public class ChannelSubscribeAction : RequestAction<ChannelSubscribe, ACKResponse> {

        public ChannelSubscribeAction(IClient pClient, ChannelSubscribe pRequest) : base(pClient, pRequest) {
        }

        public override ACKResponse Start() {
            if (new SubscriptionClient<ChannelSubscriptionHandler>(Client).Sub(Request)) {
                return new ACKResponse(Request);
            }
            return new ACKResponse(Request,
                new ResponseStatus(
                    ResponseState.Error,
                    $"Failed to subscribe to {Request.ChannelName}"
                )
            );
        }
    }
}