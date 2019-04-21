using Mitto.IMessaging;
using Mitto.Messaging.Action;
using Mitto.Messaging.Response;
using Mitto.Subscription.IMessaging.Handlers;
using Mitto.Subscription.Messaging.Request;

namespace Mitto.Subscription.Messaging.Tests.Action.Request {

    internal class SendToChannelRequestAction : RequestAction<SendToChannelRequest, ACKResponse> {

        public SendToChannelRequestAction(IClient objClient, SendToChannelRequest objMessage) : base(objClient, objMessage) {
        }

        public override ACKResponse Start() {
            if (MessagingFactory.Provider.GetSubscriptionHandler<IChannelSubscriptionHandler>().Notify(Client.Router, Request)) {
                return new ACKResponse(Request);
            }
            return new ACKResponse(Request, new ResponseStatus(ResponseState.Error, $"Failed to subscribe to {Request.ChannelName}"));
        }
    }
}