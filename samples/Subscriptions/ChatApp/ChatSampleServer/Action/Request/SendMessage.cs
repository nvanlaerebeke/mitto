using Mitto.IMessaging;
using Mitto.Messaging.Action;

namespace ChatSampleServer.Action.Request {
	public class SendMessage : RequestAction<ChatSample.Messaging.Request.SendMessage> {

		public SendMessage(IClient pClient, ChatSample.Messaging.Request.SendMessage pMessage) : base(pClient, pMessage) { }
		public override IResponseMessage Start() {
			var obj = MessagingFactory.Provider.GetSubscriptionHandler<SubscriptionHandler.Chat>();
			obj.Notify(Client, Request);
			return new Mitto.Messaging.Response.ACK(Request, ResponseCode.Success);
		}
	}
}