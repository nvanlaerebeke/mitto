using Mitto.IMessaging;

namespace Mitto.Messaging.Action.Request {
	public class SendToChannel : RequestAction<Messaging.Request.ISendToChannel> {
		public SendToChannel(IClient pClient, Messaging.Request.ISendToChannel pMessage) : base(pClient, pMessage) { }

		public override IResponseMessage Start() {
			var obj = MessagingFactory.Provider.GetSubscriptionHandler<SubscriptionHandler.IChannel>();
			if (obj.Notify(Client, Request)) {
				return new Response.ACK(Request, ResponseCode.Success);
			}
			return new Response.ACK(Request, ResponseCode.Error);
		}
	}
}