using ChatSample.Messaging.Request;
using Mitto.IMessaging;
using Mitto.Messaging.Action;
using Mitto.Messaging.Response;

namespace ChatSampleServer.Action.Request {
	public class SendMessageRequestAction : RequestAction<SendMessageRequest, ACKResponse> {

		public SendMessageRequestAction(IClient pClient, SendMessageRequest pMessage) : base(pClient, pMessage) { }
		public override IResponseMessage Start() {
			var obj = MessagingFactory.Provider.GetSubscriptionHandler<SubscriptionHandler.ChatSubscriptionHandler>();
			obj.Notify(Client, Request);
			return new ACKResponse(Request, ResponseCode.Success);
		}
	}
}