using Mitto.IMessaging;

namespace Mitto.Messaging.Action.Request {
	public class SendToChannel : RequestAction<Messaging.Request.SendToChannel> {
		public SendToChannel(IClient pClient, Messaging.Request.SendToChannel pMessage) : base(pClient, pMessage) { }

		public override IResponseMessage Start() {
			if(MessagingFactory.Provider.GetSubscriptionHandler<SubscriptionHandler.Channel>().Notify(Client, Request)) {
				return new Response.ACK(Request, ResponseCode.Success);
			}
			return new Response.ACK(Request, ResponseCode.Error);
		}
	}
}
