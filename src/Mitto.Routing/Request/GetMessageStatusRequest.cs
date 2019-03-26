using Mitto.IRouting;
using System.Text;

namespace Mitto.Routing.Request {

	public class GetMessageStatusRequest : ControlRequestMessage {
		public string RequestID { get; set; }

		public GetMessageStatusRequest(string pRequestID) {
			RequestID = pRequestID;
		}

		public GetMessageStatusRequest(ControlFrame pFrame) {
			ID = pFrame.RequestID;
			RequestID = Encoding.ASCII.GetString(pFrame.Data);
		}

		public override ControlFrame GetFrame() {
			return GetFrame(Encoding.ASCII.GetBytes(RequestID));
		}
	}
}