using Mitto.IRouting;
using System;
using System.Linq;
using System.Text;

namespace Mitto.Routing.Request {
	public class GetMessageStatusRequest : ControlRequest {
		public string RequestID { get; set; }
		public GetMessageStatusRequest(string pRequestID) {
			RequestID = pRequestID;
		}

		/// <summary>
		/// ToDo: don't use linq, it very very slow, use Array.Copy into a new array
		/// </summary>
		/// <param name="pData"></param>
		public GetMessageStatusRequest(ControlFrame pFrame) {
			ID = pFrame.RequestID;
			var arrData = pFrame.Data;
			var arrRequestID = new byte[arrData[0]];
			Array.Copy(arrData, 1, arrRequestID, 0, arrRequestID.Length);
			RequestID = Encoding.ASCII.GetString(arrRequestID);
		}

		public override ControlFrame GetFrame() {
			var arrRequestID = Encoding.ASCII.GetBytes(RequestID);
			var arrNewArray = new byte[1 + arrRequestID.Length];
			arrNewArray[0] = (byte)arrRequestID.Length;
			Array.Copy(arrRequestID, 0, arrNewArray, 1, arrRequestID.Length);
			return new ControlFrame(ControlFrameType.Request, this.GetType().Name, ID, arrNewArray);
		}
	}
}