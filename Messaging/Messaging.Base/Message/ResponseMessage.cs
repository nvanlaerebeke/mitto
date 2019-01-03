using IMessaging;
using System;

namespace Messaging.Base {
    public abstract class ResponseMessage : Message {
        public ResponseCode Status { get; set; } 
        public string Message { get; set; }
		public ResponseMessage() : base(MessageType.Response, Guid.NewGuid().ToString()) { }
        public ResponseMessage(RequestMessage pMessage, ResponseCode pStatus): base(MessageType.Response, pMessage.ID) {
			ID = pMessage.ID;
            Status = pStatus;
        }
		public ResponseMessage(RequestMessage pMessage, ResponseCode pStatus, string pStatusMessage) : base(MessageType.Response, pMessage.ID) {
			ID = pMessage.ID;
			Status = pStatus;
            Message = pStatusMessage;
		}
    }
}