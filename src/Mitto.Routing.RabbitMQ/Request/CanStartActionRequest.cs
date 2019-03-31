using Mitto.IRouting;
using Mitto.Routing.Request;
using System.Text;

namespace Mitto.Routing.RabbitMQ.Request {

	public class CanStartActionRequest : ControlRequestMessage {
		public string RequestID { get; set; }

		public CanStartActionRequest(string pRequestID) {
			RequestID = pRequestID;
		}

		public CanStartActionRequest(ControlFrame pFrame) {
			ID = pFrame.RequestID;
			RequestID = Encoding.ASCII.GetString(pFrame.Data);
		}

		public override ControlFrame GetFrame() {
			return GetFrame(Encoding.ASCII.GetBytes(RequestID));
		}
	}
}