using Mitto.IMessaging;
using System;

namespace Mitto.Messaging {
	internal class Client : IClient {
		private IQueue.IQueue _objClient;
		private RequestManager _objRequestManager;

		public Client(IQueue.IQueue pClient, RequestManager pRequestManager) {
			_objClient = pClient;
			_objRequestManager = pRequestManager;
		}

		public void Request<R>(IMessage pMessage, Action<R> pAction) where R : IResponseMessage {
			_objRequestManager.Request<R>(this, pMessage, pAction);
		}

		public void Transmit(IMessage pMessage) {
			_objClient.Transmit(MessagingFactory.Converter.GetByteArray(pMessage));
		}
	}
}