using Mitto.IMessaging;
using Mitto.Messaging.Base;
using System;

namespace Messaging.App.Response {
    public class Echo: ResponseMessage {
		public string Response { get; set; }
        public Echo() { }
        public Echo(Request.Echo pMessage, ResponseCode pStatus) : base(pMessage, pStatus) {
			Response = pMessage.Message;
		}

        public override byte GetCode() {
            return 0x31;
        }
    }
}