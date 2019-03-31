using Mitto.IRouting;
using System.Text;

namespace Mitto.Routing.Request {

	public class PingRequest : ControlRequestMessage {
		public PingRequest() {}

		public PingRequest(ControlFrame pFrame) { }

		public override ControlFrame GetFrame() {
			return GetFrame(new byte[] { });
		}
	}
}