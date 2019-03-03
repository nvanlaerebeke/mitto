using ChatSample.Messaging;
using ChatSample.Messaging.Subscribe;
using Mitto.IMessaging;
using Mitto.Messaging.Action;
using Mitto.Messaging.Response;

namespace ChatSampleServer.Action.Subscribe {
	public class ChatSubscribeAction : RequestAction<ChatSubscribe, ACKResponse> {

		public ChatSubscribeAction(IClient pClient, ChatSubscribe pMessage) : base(pClient, pMessage) { }
		public override IResponseMessage Start() {
			var obj = MessagingFactory.Provider.GetSubscriptionHandler<SubscriptionHandler.ChatSubscriptionHandler>();
			if(obj.Sub(Client, Request)) {
				return new ACKResponse(Request);
			}
			return new ACKResponse(Request, new ResponseStatus(ResponseState.Error, (int)CustomResponseCode.ChatSubscribeActionFailed));
		}
	}
}