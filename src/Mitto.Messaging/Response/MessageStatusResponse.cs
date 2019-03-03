using Mitto.IMessaging;

namespace Mitto.Messaging.Response {
    public class MessageStatusResponse: ResponseMessage {
        public MessageStatusResponse() { }
		public MessageStatusResponse(IRequestMessage pRequest, ResponseStatus pStatus) : base(pRequest, pStatus) { }
		public MessageStatusResponse(IRequestMessage pRequest, MessageStatusType pStatus) : base(pRequest) {
			RequestStatus = pStatus;
		}
		public MessageStatusType RequestStatus { get; set; } = MessageStatusType.UnKnown;
	}
}