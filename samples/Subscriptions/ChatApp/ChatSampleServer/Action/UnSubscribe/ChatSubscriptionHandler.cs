using ChatSample.Messaging;
using ChatSample.Messaging.UnSubscribe;
using Mitto.IMessaging;
using Mitto.Messaging.Action;
using Mitto.Messaging.Response;

namespace ChatSampleServer.Action.UnSubscribe {
	public class ChatSubscriptionHandler : RequestAction<ChatUnSubscribe, ACKResponse> {

		public ChatSubscriptionHandler(IClient pClient, ChatUnSubscribe pMessage) : base(pClient, pMessage) { }
		public override IResponseMessage Start() {
			var obj = MessagingFactory.Provider.GetSubscriptionHandler<SubscriptionHandler.ChatSubscriptionHandler>();
			if (obj.UnSub(Client, Request)) {
				return new ACKResponse(Request);
			}
			return new ACKResponse(Request, new ResponseStatus(ResponseState.Error, (int)CustomResponseCode.ChatUnSubscribeActionFailed));
		}
	}
}