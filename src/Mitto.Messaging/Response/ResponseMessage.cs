using Mitto.IMessaging;
using System;

namespace Mitto.Messaging {
    public abstract class ResponseMessage : Message, IResponseMessage {
        public ResponseStatus Status { get; set; } 
		public ResponseMessage() : base(MessageType.Response, Guid.NewGuid().ToString()) { }

		public ResponseMessage(IRequestMessage pMessage) : base(MessageType.Response, pMessage.ID) {
			Status = new ResponseStatus();
		}

        public ResponseMessage(IRequestMessage pMessage, ResponseStatus pStatus): base(MessageType.Response, pMessage.ID) {
			Status = pStatus;
        }
    }
}