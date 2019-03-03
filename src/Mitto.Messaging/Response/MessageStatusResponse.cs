using Mitto.IMessaging;

namespace Mitto.Messaging.Response {
    public class MessageStatusResponse: ResponseMessage {
        public MessageStatusResponse() { }
		public MessageStatusResponse(IRequestMessage pRequest, ResponseCode pCode) : base(pRequest, pCode) { }
		public MessageStatusResponse(IRequestMessage pRequest, MessageStatusType pStatus) : base(pRequest, ResponseCode.Success) {
			RequestStatus = pStatus;
		}
		public MessageStatusType RequestStatus { get; set; } = MessageStatusType.UnKnown;
	}
}