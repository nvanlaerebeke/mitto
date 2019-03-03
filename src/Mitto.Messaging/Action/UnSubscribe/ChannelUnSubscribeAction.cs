using Mitto.IMessaging;
using Mitto.Messaging.UnSubscribe;
using Mitto.Messaging.Response;

namespace Mitto.Messaging.Action.UnSubscribe {
	public class ChannelUnSubscribeAction : RequestAction<ChannelUnSubscribe, ACKResponse> {

		public ChannelUnSubscribeAction(IClient pClient, Messaging.UnSubscribe.ChannelUnSubscribe pRequest) : base (pClient, pRequest) { }

		public override IResponseMessage Start() {
			if (MessagingFactory.Provider.GetSubscriptionHandler<SubscriptionHandler.IChannelSubscriptionHandler>().UnSub(Client, Request)) {
				return new Response.ACKResponse(Request, ResponseCode.Success);
			}
			return new Response.ACKResponse(Request, ResponseCode.Error);
		}
	}
}