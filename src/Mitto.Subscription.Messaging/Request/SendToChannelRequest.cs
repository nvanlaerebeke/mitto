using Mitto.Messaging;
using Mitto.Subscription.IMessaging.Request;

namespace Mitto.Subscription.Messaging.Request {
	public class SendToChannelRequest : RequestMessage, ISendToChannelRequest {
		public string ChannelName { get; set; }
		public string Message { get; set; }

		public SendToChannelRequest(string pName, string pMessage) {
			ChannelName = pName;
			Message = pMessage;
		}
	}
}