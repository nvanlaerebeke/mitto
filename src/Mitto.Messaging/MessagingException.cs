using Mitto.IMessaging;
using Mitto.IRouting;
using System;

namespace Mitto.Messaging {
	public class MessagingException : Exception {
		public ResponseStatus Status { get; private set; }
        public int ErrorCode { get; private set; } = 0;
		public MessagingException(ResponseStatus pStatus) : base(pStatus.Message) {
			Status = pStatus;
		}
	}
}