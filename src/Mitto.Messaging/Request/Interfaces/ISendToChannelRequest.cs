using Mitto.IRouting;

namespace Mitto.Messaging.Request {
	public interface ISendToChannelRequest : IRequestMessage {
		string ChannelName { get; set; }
		string Message { get; set; }
	}
}