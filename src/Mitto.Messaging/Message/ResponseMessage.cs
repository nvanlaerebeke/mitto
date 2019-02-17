using Mitto.IMessaging;
using System;

namespace Mitto.Messaging {
	/// <summary>
	/// ToDo: Request should be the actual request message type and not the base class
	/// Already done for Action but not for ResponseMessage
	/// </summary>
    public abstract class ResponseMessage : Message {
        public ResponseCode Status { get; set; } 
		public ResponseMessage() : base(MessageType.Response, Guid.NewGuid().ToString()) { }
		protected RequestMessage Request { get; private set; }

        public ResponseMessage(RequestMessage pMessage, ResponseCode pStatus): base(MessageType.Response, pMessage.ID) {
			Request = pMessage;
            Status = pStatus;
        }
    }
}