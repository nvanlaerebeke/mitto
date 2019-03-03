using Mitto.IMessaging;
using Mitto.Messaging.Action;

namespace ChatSampleClient.Action.Request {
	public delegate void ChatMessageReceived(string pChannel, string pMessage);

	public class ReceiveMessage : RequestAction<ChatSample.Messaging.Request.ReceiveMessage> {
		public static event ChatMessageReceived MessageReceived;
		public ReceiveMessage(IClient pClient, ChatSample.Messaging.Request.ReceiveMessage pMessage) : base(pClient, pMessage) { }
		public override IResponseMessage Start() {
			MessageReceived?.Invoke(Request.Channel, Request.Message);
			return new Mitto.Messaging.Response.ACK(Request, ResponseCode.Success);
		}
	}
}