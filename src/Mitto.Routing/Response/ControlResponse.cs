using Mitto.IRouting;

namespace Mitto.Routing.Response {
	public abstract class ControlResponse :  ControlMessage, IControlResponse {
		public ControlResponse(string pRequestID) : base(MessageType.Response) {
			ID = pRequestID;
		}
	}
}