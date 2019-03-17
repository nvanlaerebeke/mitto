namespace Mitto.Routing.Response {
	public abstract class ControlResponse :  ControlMessage, IControlResponse {
		public string ID { get; private set; }
		public ControlResponse(string pRequestID) {
			ID = pRequestID;
		}
	}
}