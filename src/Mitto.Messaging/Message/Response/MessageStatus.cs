using Mitto.IMessaging;

namespace Mitto.Messaging.Response {
    public class MessageStatus: ResponseMessage {
        public MessageStatus() { }
		public MessageStatus(IMessage pRequest, ResponseCode pCode) : base(pRequest, pCode) { }
		public MessageStatus(IMessage pRequest, MessageStatusType pStatus) : base(pRequest, ResponseCode.Success) {
			RequestStatus = pStatus;
		}
		public MessageStatusType RequestStatus { get; set; } = MessageStatusType.UnKnown;
	}
}