using Mitto.IRouting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mitto.Routing {

	public abstract class ControlMessage : IControlMessage {
		public string ID { get; protected set; } = System.Guid.NewGuid().ToString();

		public abstract ControlFrame GetFrame();

		protected ControlFrame GetFrame(byte[] pData) {
			return new ControlFrame(ControlFrameType.Request, this.GetType().Name, ID, pData);
		}
	}
}