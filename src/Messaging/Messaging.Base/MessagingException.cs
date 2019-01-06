using System;

namespace Messaging.Base {
	public class MessagingException : Exception {
		public ResponseCode Code { get; private set; }
		public MessagingException(ResponseCode pCode) : base(pCode.ToString()) {
			Code = pCode;
		}
	}
}