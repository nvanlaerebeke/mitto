using Mitto.IMessaging;
using System;

namespace Mitto.Messaging {
	public abstract class SubMessage : Message, IRequestMessage {
		public SubMessage() : base(MessageType.Sub, Guid.NewGuid().ToString()) { }
		public SubMessage(MessageType pType) : base(pType, Guid.NewGuid().ToString()) { }
	}
}