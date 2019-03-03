using Mitto.IMessaging;

namespace Mitto.Messaging.Action.Subscribe {
	public class Channel : RequestAction<Messaging.Subscribe.Channel> {

		public Channel(IClient pClient, Messaging.Subscribe.Channel pRequest) : base (pClient, pRequest) { }

		public override IResponseMessage Start() {
			if(MessagingFactory.Provider.GetSubscriptionHandler<SubscriptionHandler.IChannel>().Sub(Client, Request)) {
				return new Response.ACK(Request, ResponseCode.Success);
			}
			return new Response.ACK(Request, ResponseCode.Error);
		}
	}
}