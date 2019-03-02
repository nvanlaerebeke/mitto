using System;

namespace Mitto.IMessaging {
	public interface IClient {
		void Transmit(IMessage pMessage);
		void Request<R>(IMessage pMessage, Action<R> pAction) where R : IResponseMessage;

		IQueue.IQueue Queue { get; }
	}
}