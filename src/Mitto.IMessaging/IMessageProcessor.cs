﻿using System;

namespace Mitto.IMessaging {
	public interface IMessageProcessor {
		void Process(IQueue.IQueue pQueue, byte[] pData);
		void Request<T>(IQueue.IQueue pClient, IMessage pMessage, Action<T> pAction) where T : IResponseMessage;
		MessageStatusType GetStatus(string pRequestID);
	}
}