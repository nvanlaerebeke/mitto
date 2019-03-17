using Mitto.IRouting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mitto.Routing {
	public abstract class ControlMessage : IControlMessage {
		public abstract ControlFrame GetFrame();
	}
}
