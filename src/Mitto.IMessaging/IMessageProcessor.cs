namespace Mitto.IMessaging {
	public interface IMessageProcessor {
		void Process(IQueue.IQueue pQueue, IQueue.Message pMessage);
	}
}