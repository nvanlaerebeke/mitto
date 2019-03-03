using Mitto.IMessaging;
using Mitto.Messaging.Action;

namespace ChatSampleServer.Action.Subscribe {
	public class Chat : RequestAction<ChatSample.Messaging.Subscribe.Chat> {

		public Chat(IClient pClient, ChatSample.Messaging.Subscribe.Chat pMessage) : base(pClient, pMessage) { }
		public override IResponseMessage Start() {
			var obj = MessagingFactory.Provider.GetSubscriptionHandler<SubscriptionHandler.Chat>();
			obj.Sub(Client, Request);
			return new Mitto.Messaging.Response.ACK(Request, ResponseCode.Success);
		}
	}
}