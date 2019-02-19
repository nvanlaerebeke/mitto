using Mitto.IMessaging;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mitto.Messaging.Tests")]
namespace Mitto.Messaging {
	public class MessageProcessor : IMessageProcessor {
		internal IRequestManager RequestManager { get; set; } = new RequestManager();

		/// <summary>
		/// Takes in byte[] data, fetches the message it represents and passes
		/// that message to the RequestManager to processes said message
		/// </summary>
		/// <param name="pClient"></param>
		/// <param name="pData"></param>
		public void Process(IQueue.IQueue pClient, byte[] pData) {
			IMessage objMessage = MessagingFactory.Converter.GetMessage(pData);
			// --- don't know what we're receiving, so skip it
			if (objMessage == null) { return; }
			RequestManager.Process(pClient, objMessage);
		}
	}
}