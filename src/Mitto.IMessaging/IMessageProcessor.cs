using Mitto.IRouting;
using System;

namespace Mitto.IMessaging {
	public interface IMessageProcessor {
		void Process(IRouter pRouter, byte[] pData);
		void Request<T>(IRouter pRouter, IRequestMessage pMessage, Action<T> pAction) where T : IResponseMessage;
		MessageStatus GetStatus(string pRequestID);
	}
}