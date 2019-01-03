using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Base {
	public class MessagingException : Exception {
		public ResponseCode Code { get; private set; }
		public MessagingException(ResponseCode pCode) : base(pCode.ToString()) {
			Code = pCode;
		}
	}
}