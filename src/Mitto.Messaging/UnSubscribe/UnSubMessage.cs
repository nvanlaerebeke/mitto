using Mitto.IMessaging;
using System;

namespace Mitto.Messaging {
	public abstract class UnSubMessage : Message, IRequestMessage {
		public UnSubMessage() : base(MessageType.UnSub, Guid.NewGuid().ToString()) { }
		public UnSubMessage(MessageType pType) : base(pType, Guid.NewGuid().ToString()) { }
	}
}