using Mitto.IMessaging;

namespace Mitto.Messaging.Response {
    public class Echo: ResponseMessage {
        public Echo() { }
        public Echo(IMessage pMessage, ResponseCode pStatus) : base(pMessage, pStatus) { }

		public string Message {
			get {
				return ((Messaging.Request.Echo)Request).Message;
			}
		}
    }
}