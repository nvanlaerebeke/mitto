using Mitto.IMessaging;
using Mitto.Messaging.Action;
using Mitto.Messaging.Response;
using Mitto.Subscription.Messaging.Handlers;
using Mitto.Subscription.Messaging.UnSubscribe;

namespace Mitto.Subscription.Messaging.Action.UnSubscribe {

    public class ChannelUnSubscribeAction : RequestAction<ChannelUnSubscribe, ACKResponse> {

        public ChannelUnSubscribeAction(IClient pClient, ChannelUnSubscribe pRequest) : base(pClient, pRequest) {
        }

        public override ACKResponse Start() {
            if (new SubscriptionClient<ChannelSubscriptionHandler>(Client).UnSub(Request)) {
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