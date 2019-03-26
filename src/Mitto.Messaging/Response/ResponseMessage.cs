using Mitto.IMessaging;
using Mitto.IRouting;
using System;

namespace Mitto.Messaging {
    public abstract class ResponseMessage : Message, IResponseMessage {
		public DateTime StartTime { get; set; }
		public DateTime EndTime => DateTime.Now;
		public ResponseStatus Status { get; set; } 
		public ResponseMessage() : base(MessageType.Response, Guid.NewGuid().ToString()) { }

		public ResponseMessage(IRequestMessage pMessage) : base(MessageType.Response, pMessage.ID) {
			StartTime = pMessage.StartTime;
			Status = new ResponseStatus();
		}

        public ResponseMessage(IRequestMessage pMessage, ResponseStatus pStatus): base(MessageType.Response, pMessage.ID) {
			StartTime = pMessage.StartTime;
			Status = pStatus;
        }
    }
}