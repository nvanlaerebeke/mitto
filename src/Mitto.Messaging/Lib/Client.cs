using Mitto.IMessaging;
using System;

namespace Mitto.Messaging {
	/// <summary>
	/// Represents an easy to use interface to communicate with the IQueue.IQueue
	/// </summary>
	internal class Client : IClient {
		private IQueue.IQueue _objClient;
		private IRequestManager _objRequestManager;

		public IQueue.IQueue Queue {
			get { return _objClient; }
		}

		public Client(IQueue.IQueue pClient, IRequestManager pRequestManager) {
			_objClient = pClient;
			_objRequestManager = pRequestManager;
		}

		/// <summary>
		/// Sends a request over the IQueue connection and runs the
		/// action when the response is received
		/// </summary>
		/// <typeparam name="R"></typeparam>
		/// <param name="pMessage"></param>
		/// <param name="pAction"></param>
		public void Request<R>(IRequestMessage pMessage, Action<R> pAction) where R : IResponseMessage {
			_objRequestManager.Request<R>(new Request<R>(this, pMessage, pAction));
		}

		/// <summary>
		/// Transmits an IMessage over the IQueue connection
		/// Nothing as response is expected
		/// </summary>
		/// <param name="pMessage"></param>
		public void Transmit(IMessage pMessage) {
			_objClient.Transmit(new Frame(
				pMessage.Type, 
				pMessage.Name, 
				MessagingFactory.Converter.GetByteArray(pMessage)
			).GetByteArray());
		}
	}
}