using Mitto.IMessaging;

namespace Mitto.Messaging.Action.Request {
	public delegate void ChannelMessageReceived(string pChannel, string pMessage);

	public class ReceiveOnChannel : RequestAction<Messaging.Request.ReceiveOnChannel> {
		public static event ChannelMessageReceived ChannelMessageReceived;
		public ReceiveOnChannel(IClient pClient, Messaging.Request.ReceiveOnChannel pMessage) : base(pClient, pMessage) { }

		public override IResponseMessage Start() {
			ChannelMessageReceived?.Invoke(Request.ChannelName, Request.Message);
			return new Response.ACK(Request, ResponseCode.Success);
		}
	}
}
