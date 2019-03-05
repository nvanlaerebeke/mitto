using Mitto.IMessaging;
using System;

namespace Mitto.Messaging {
	public class MessagingException : Exception {
		public ResponseStatus Status { get; private set; }
		public MessagingException(ResponseStatus pStatus) : base(pStatus.Message) {
			Status = pStatus;
		}
	}
}