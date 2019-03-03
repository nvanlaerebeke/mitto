using Mitto.IMessaging;
using System;

namespace Mitto.Messaging {
	public class MessagingException : Exception {
		public ResponseState Code { get; private set; }
		public MessagingException(ResponseState pCode) : base(pCode.ToString()) {
			Code = pCode;
		}
	}
}