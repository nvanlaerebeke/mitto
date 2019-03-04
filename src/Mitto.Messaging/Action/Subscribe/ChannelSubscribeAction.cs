using Mitto.IMessaging;
using Mitto.Messaging.Action.SubscriptionHandler;
using Mitto.Messaging.Response;
using Mitto.Messaging.Subscribe;

namespace Mitto.Messaging.Action.Subscribe {
	public class ChannelSubscribeAction : RequestAction<ChannelSubscribe, ACKResponse> {

		public ChannelSubscribeAction(IClient pClient, ChannelSubscribe pRequest) : base (pClient, pRequest) { }

		public override ACKResponse Start() {
			if(MessagingFactory.Provider.GetSubscriptionHandler<IChannelSubscriptionHandler>().Sub(Client, Request)) {
				return new ACKResponse(Request);
			}
			return new ACKResponse(Request, new ResponseStatus(ResponseState.Error, $"Failed to subscribe to {Request.ChannelName}"));
		}
	}
}