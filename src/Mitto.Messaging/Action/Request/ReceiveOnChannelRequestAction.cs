using Mitto.IMessaging;
using Mitto.Messaging.Request;
using Mitto.Messaging.Response;

namespace Mitto.Messaging.Action.Request {
	public delegate void ChannelMessageReceived(string pChannel, string pMessage);

	public class ReceiveOnChannelRequestAction : RequestAction<ReceiveOnChannelRequest, ACKResponse> {
		public static event ChannelMessageReceived ChannelMessageReceived;
		public ReceiveOnChannelRequestAction(IClient pClient, ReceiveOnChannelRequest pMessage) : base(pClient, pMessage) { }

		public override IResponseMessage Start() {
			ChannelMessageReceived?.Invoke(Request.ChannelName, Request.Message);
			return new ACKResponse(Request, ResponseCode.Success);
		}
	}
}
