using Mitto.IMessaging;
using System;

namespace Mitto.Messaging {
	public class MessagingException : Exception {
		public ResponseCode Code { get; private set; }
		public MessagingException(ResponseCode pCode) : base(pCode.ToString()) {
			Code = pCode;
		}
	}
}