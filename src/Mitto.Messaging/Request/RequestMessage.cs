using Mitto.IMessaging;
using Mitto.IRouting;
using System;

namespace Mitto.Messaging {
	public abstract class RequestMessage : Message, IRequestMessage {
		public DateTime StartTime { get; set; } = DateTime.Now;
		public RequestMessage() : this(MessageType.Request) { }
		public RequestMessage(MessageType pType) : base(pType, Guid.NewGuid().ToString()) { }
	}
}
