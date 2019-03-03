using ChatSample.Messaging.Request;
using Mitto.IMessaging;
using Mitto.Messaging.Action;
using Mitto.Messaging.Response;

namespace ChatSampleClient.Action.Request {
	public delegate void ChatMessageReceived(string pChannel, string pMessage);

	public class ReceiveMessageRequestAction : RequestAction<ReceiveMessageRequest, ACKResponse> {
		public static event ChatMessageReceived MessageReceived;
		public ReceiveMessageRequestAction(IClient pClient, ReceiveMessageRequest pMessage) : base(pClient, pMessage) { }
		public override IResponseMessage Start() {
			MessageReceived?.Invoke(Request.Channel, Request.Message);
			return new ACKResponse(Request, ResponseCode.Success);
		}
	}
}