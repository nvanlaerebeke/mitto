using Mitto.IMessaging;
using Mitto.Messaging.Action;

namespace ChatSampleServer.Action.UnSubscribe {
	public class Chat : RequestAction<ChatSample.Messaging.UnSubscribe.Chat> {

		public Chat(IClient pClient, ChatSample.Messaging.UnSubscribe.Chat pMessage) : base(pClient, pMessage) { }
		public override IResponseMessage Start() {
			var obj = MessagingFactory.Provider.GetSubscriptionHandler<SubscriptionHandler.Chat>();
			obj.UnSub(Client, Request);
			return new Mitto.Messaging.Response.ACK(Request, ResponseCode.Success);
		}
	}
}