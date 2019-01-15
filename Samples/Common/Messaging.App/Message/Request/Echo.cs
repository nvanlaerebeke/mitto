
using Mitto.Messaging.Base;

namespace Messaging.App.Request {
    public class Echo : RequestMessage {
		public string Message { get; set; }

		public Echo() : base() { }
        public Echo(string pMessage) : base() {
			Message = pMessage;
		}

        public override byte GetCode() {
            return 0x30;
        }
    }
}