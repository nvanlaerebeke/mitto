using Mitto.IMessaging;
using System;

namespace Mitto.Messaging {
	/// <summary>
	/// Represents an easy to use interface to communicate with the IQueue.IQueue
	/// </summary>
	internal class Client : IClient, IEquatable<Client> {
		public string ID { get { return Queue.ID; } } 
		private IRequestManager RequestManager { get; set; }
		private IQueue.IQueue Queue { get; set; }

		public Client(IQueue.IQueue pClient, IRequestManager pRequestManager) {
			Queue = pClient;
			RequestManager = pRequestManager;
		}

		/// <summary>
		/// Sends a request over the IQueue connection and runs the
		/// action when the response is received
		/// </summary>
		/// <typeparam name="R"></typeparam>
		/// <param name="pMessage"></param>
		/// <param name="pAction"></param>
		public void Request<R>(IRequestMessage pMessage, Action<R> pAction) where R : IResponseMessage {
			RequestManager.Request<R>(new Request<R>(this, pMessage, pAction));
		}

		/// <summary>
		/// Transmits an IMessage over the IQueue connection
		/// Nothing as response is expected
		/// </summary>
		/// <param name="pMessage"></param>
		public void Transmit(IMessage pMessage) {
			Queue.Transmit(new Frame(
				pMessage.Type, 
				pMessage.Name, 
				MessagingFactory.Converter.GetByteArray(pMessage)
			).GetByteArray());
		}

		public bool Equals(Client pClient) {
			return (
				this.ID == pClient.ID
			);
		}

		public bool Equals(IClient pClient) {
			return (
				this.ID == pClient.ID
			);
		}
	}
}