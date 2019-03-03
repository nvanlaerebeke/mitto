﻿using Mitto.IMessaging;

namespace Mitto.Messaging.Response {
    public class MessageStatus: ResponseMessage {
        public MessageStatus() { }
		public MessageStatus(IRequestMessage pRequest, ResponseCode pCode) : base(pRequest, pCode) { }
		public MessageStatus(IRequestMessage pRequest, MessageStatusType pStatus) : base(pRequest, ResponseCode.Success) {
			RequestStatus = pStatus;
		}
		public MessageStatusType RequestStatus { get; set; } = MessageStatusType.UnKnown;
	}
}