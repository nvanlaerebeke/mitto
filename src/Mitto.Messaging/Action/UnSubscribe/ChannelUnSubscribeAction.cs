using Mitto.IMessaging;
using Mitto.Messaging.UnSubscribe;
using Mitto.Messaging.Response;
using Mitto.IRouting;

namespace Mitto.Messaging.Action.UnSubscribe {
	public class ChannelUnSubscribeAction : RequestAction<ChannelUnSubscribe, ACKResponse> {

		public ChannelUnSubscribeAction(IClient pClient, ChannelUnSubscribe pRequest) : base (pClient, pRequest) { }

		public override ACKResponse Start() {
			if (MessagingFactory.Provider.GetSubscriptionHandler<SubscriptionHandler.IChannelSubscriptionHandler>().UnSub(Client, Request)) {
				return new ACKResponse(Request);
			}
			return new ACKResponse(Request, new ResponseStatus(ResponseState.Error, $"Failed to unsubscribe from {Request.ChannelName}"));
		}
	}
}