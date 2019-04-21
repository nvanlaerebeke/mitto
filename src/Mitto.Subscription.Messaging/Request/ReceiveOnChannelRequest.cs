using Mitto.Messaging;
using Mitto.Subscription.IMessaging.Request;

namespace Mitto.Subscription.Messaging.Request {
	public class ReceiveOnChannelRequest : RequestMessage, IReceiveOnChannelRequest {
		public string ChannelName { get; set; }
		public string Message { get; set; }

		public ReceiveOnChannelRequest(string pName, string pMessage) {
			ChannelName = pName;
			Message = pMessage;
		}
	}
}