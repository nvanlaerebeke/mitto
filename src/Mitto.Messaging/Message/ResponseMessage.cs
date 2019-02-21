using Mitto.IMessaging;
using System;

namespace Mitto.Messaging {
	/// <summary>
	/// ToDo: Request should be the actual request message type and not the base class
	/// Already done for Action but not for ResponseMessage
	/// Also, Request should not be public, do we even need to keep the request in the response?
	/// See where this would be usefull, but think we can safely remove it. - only location a response
	/// would be usefull is where the request was made meaning we have the info already
	/// </summary>
    public abstract class ResponseMessage : Message, IResponseMessage {
        public ResponseCode Status { get; set; } 
		public ResponseMessage() : base(MessageType.Response, Guid.NewGuid().ToString()) { }
		public IMessage Request { get; private set; }

        public ResponseMessage(IMessage pMessage, ResponseCode pStatus): base(MessageType.Response, pMessage.ID) {
			Request = pMessage;
            Status = pStatus;
        }
    }
}