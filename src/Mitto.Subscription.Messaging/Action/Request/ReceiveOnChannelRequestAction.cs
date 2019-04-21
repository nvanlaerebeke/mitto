using Mitto.IMessaging;
using Mitto.Messaging.Action;
using Mitto.Messaging.Response;
using Mitto.Subscription.Messaging.Request;

namespace Mitto.Subscription.Messaging.Action.Request {

    public delegate void ChannelMessageReceived(string pChannel, string pMessage);

    public class ReceiveOnChannelRequestAction : RequestAction<
        ReceiveOnChannelRequest,
        ACKResponse
    > {

        public static event ChannelMessageReceived ChannelMessageReceived;

        public ReceiveOnChannelRequestAction(IClient pClient, ReceiveOnChannelRequest pMessage) : base(pClient, pMessage) {
        }

        public override ACKResponse Start() {
            ChannelMessageReceived?.Invoke(Request.ChannelName, Request.Message);
            return new ACKResponse(Request);
        }
    }
}