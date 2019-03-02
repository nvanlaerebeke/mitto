using System;

namespace Mitto.IMessaging {
	public interface IMessageProcessor {
		void Process(IQueue.IQueue pQueue, byte[] pData);
		void Request<T>(IQueue.IQueue pClient, IRequestMessage pMessage, Action<T> pAction) where T : IResponseMessage;
		MessageStatusType GetStatus(string pRequestID);
	}
}