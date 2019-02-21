using Mitto.IMessaging;

namespace Mitto.Messaging {
	public interface IClient {
		void Transmit(IMessage pMessage);
	}
}