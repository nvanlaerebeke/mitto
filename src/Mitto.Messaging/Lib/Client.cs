﻿using Mitto.IMessaging;
using System;

namespace Mitto.Messaging {
	/// <summary>
	/// Represents an easy to use interface to communicate with the IQueue.IQueue
	/// </summary>
	internal class Client : IClient {
		private IQueue.IQueue _objClient;
		private IRequestManager _objRequestManager;

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
		public void Request<R>(IMessage pMessage, Action<R> pAction) where R : IResponseMessage {
			_objRequestManager.Request(_objClient, pMessage, pAction);
		}

		/// <summary>
		/// Transmits an IMessage over the IQueue connection
		/// Nothing as response is expected
		/// </summary>
		/// <param name="pMessage"></param>
		public void Transmit(IMessage pMessage) {
			_objClient.Transmit(MessagingFactory.Converter.GetByteArray(pMessage));
		}
	}
}