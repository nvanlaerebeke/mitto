using Mitto.IRouting;
using System;

namespace Mitto.Routing.Request {

	public abstract class ControlRequestMessage : ControlMessage, IControlRequest {
		public ControlRequestMessage() : base(MessageType.Request) { }
	}
}