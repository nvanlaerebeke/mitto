using Mitto.IMessaging;

namespace Mitto.Messaging.Response {
    public class Echo: ResponseMessage {
        public Echo() { }
        public Echo(RequestMessage pMessage, ResponseCode pStatus) : base(pMessage, pStatus) { }

		public string Message {
			get {
				return ((Messaging.Request.Echo)Request).Message;
			}
		}

        public override byte GetCode() {
            return 0x57;
        }
    }
}