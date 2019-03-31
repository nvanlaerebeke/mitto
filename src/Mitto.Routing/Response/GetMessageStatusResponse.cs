using Mitto.IRouting;
using Mitto.Routing.Request;

namespace Mitto.Routing.Response {
	public class GetMessageStatusResponse : ControlResponse {
		public bool IsAlive { get; set; }
		
		public GetMessageStatusResponse(GetMessageStatusRequest pRequest, bool pIsAlive) : base(pRequest.ID) {
			IsAlive = pIsAlive;
		}
		public GetMessageStatusResponse(ControlFrame pFrame) : base(pFrame.RequestID){
			IsAlive = (pFrame.Data[0] == 1);
		}

		public override ControlFrame GetFrame() {
			return GetFrame(new byte[] { (byte)(IsAlive ? 1 : 0) });
		}
	}
}