using Mitto.IMessaging;
using Mitto.Messaging.Request;

namespace Mitto.Messaging.Response {
    public class MessageStatusResponse: ResponseMessage {
		public string RequestID { get; set; }
		public MessageStatusType RequestStatus { get; set; } = MessageStatusType.UnKnown;


		public MessageStatusResponse() { }
		public MessageStatusResponse(IMessageStatusRequest pRequest, ResponseStatus pStatus) : base(pRequest, pStatus) { }
		public MessageStatusResponse(IMessageStatusRequest pRequest, MessageStatusType pStatus) : base(pRequest) {
			RequestID = pRequest.RequestID;
			RequestStatus = pStatus;
		}
	}
}