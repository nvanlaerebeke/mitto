namespace Mitto.IMessaging {
	public interface IClient {
		void Transmit(IMessage pMessage);
	}
}