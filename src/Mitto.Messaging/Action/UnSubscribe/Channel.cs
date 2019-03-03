using Mitto.IMessaging;

namespace Mitto.Messaging.Action.UnSubscribe {
	public class Channel : RequestAction<Messaging.UnSubscribe.Channel> {

		public Channel(IClient pClient, Messaging.UnSubscribe.Channel pRequest) : base (pClient, pRequest) { }

		public override IResponseMessage Start() {
			if (MessagingFactory.Provider.GetSubscriptionHandler<SubscriptionHandler.IChannel>().UnSub(Client, Request)) {
				return new Response.ACK(Request, ResponseCode.Success);
			}
			return new Response.ACK(Request, ResponseCode.Error);
		}
	}
}