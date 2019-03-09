using Mitto.IMessaging;
using System;

namespace Mitto.Messaging {
	public abstract class SubMessage : RequestMessage {
		public SubMessage() : this(MessageType.Sub) { }
		public SubMessage(MessageType pType) : base(pType) { }
	}
}