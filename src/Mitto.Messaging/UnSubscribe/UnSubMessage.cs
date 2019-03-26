using Mitto.IMessaging;
using Mitto.IRouting;
using System;

namespace Mitto.Messaging {
	public abstract class UnSubMessage : RequestMessage  {
		public UnSubMessage() : this(MessageType.UnSub) { }
		public UnSubMessage(MessageType pType) : base(pType) { }
	}
}