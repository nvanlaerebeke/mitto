using Mitto.IRouting;
using Mitto.Routing.Response;
using System;
using System.Threading.Tasks;

namespace Mitto.Routing {
	public class Request<T> : IRequest where T : ControlResponse {
		public readonly IRouter Connection;

		public string ID {
			get {
				return ControlRequest.ID;
			}
		}
		public readonly Request.ControlRequest ControlRequest;
		public readonly Action<T> Callback;

		public Request(IRouter pConnection, Request.ControlRequest pRequest, Action<T> pAction) {
			Connection = pConnection;
			ControlRequest = pRequest;
			Callback = pAction;
		}

		public void Send() {
			var objControlFrame = ControlRequest.GetFrame();
			var objRoutingFrame = new RoutingFrame(RoutingFrameType.Control, Connection.ConnectionID, objControlFrame.GetBytes());
			Connection.Transmit(objRoutingFrame.GetBytes());
		}

		public void Set(ControlFrame pFrame) {
			Task.Run(() => {
				var objResponse = ControlFactory.Provider.GetMessage(pFrame);
				Callback.DynamicInvoke(objResponse);
			});
		}
	}
}