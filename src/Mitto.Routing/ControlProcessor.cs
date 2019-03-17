using Mitto.Routing.Request;
using Mitto.Routing.Response;
using System;
using Mitto.IRouting;
using Mitto.Routing.Action;

namespace Mitto.Routing {
	public class ControlProcessor {
		private ActionManager ActionManager = new ActionManager();
		private RequestManager RequestManager = new RequestManager();

		public void Request<T>(IRouter pConnection, ControlRequest pRequest, Action<T> pAction) where T: ControlResponse {
			RequestManager.Send(new Request<T>(pConnection, pRequest, pAction));
		}

		public void Process(IRouter pConnection, ControlFrame pFrame) {
			if(pFrame.FrameType == ControlFrameType.Request) {
				ActionManager.Process(pConnection, pFrame);
			} else {
				RequestManager.Process(pFrame);
			}
		}
	}
}