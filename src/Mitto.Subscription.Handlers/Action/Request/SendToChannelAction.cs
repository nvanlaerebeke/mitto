using Mitto.IMessaging;
using Mitto.Messaging.Action;
using Mitto.Messaging.Response;
using Mitto.Subscription.Messaging.Handlers;
using Mitto.Subscription.Messaging.Request;

namespace Mitto.Subscription.Messaging.Action.Request {

    public class SendToChannelAction : RequestAction<
        SendToChannelRequest,
        ACKResponse
    > {

        public SendToChannelAction(IClient pClient, SendToChannelRequest pMessage) : base(pClient, pMessage) {
        }

        public override ACKResponse Start() {
            if (new SubscriptionClient<ChannelSubscriptionHandler>(Client).Notify(Request)) {
                return new ACKResponse(Request);
            }
            return new ACKResponse(Request,
                new ResponseStatus(
                    ResponseState.Error, $"Failed to subscribe to {Request.ChannelName}"
                )
            );
        }
    }
}