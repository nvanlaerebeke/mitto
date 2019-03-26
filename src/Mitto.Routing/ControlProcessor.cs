using Mitto.Routing.Request;
using Mitto.Routing.Response;
using System;
using Mitto.IRouting;
using Mitto.Routing.Action;

namespace Mitto.Routing {

	public class ControlProcessor {
		private readonly ActionManager ActionManager;
		private readonly RequestManager RequestManager;

		public ControlProcessor() {
			ActionManager = new ActionManager();
			RequestManager = new RequestManager();
		}

		public void Request(IRequest pRequest) {
			RequestManager.Send(pRequest);
		}

		public void Process(IRouter pConnection, RoutingFrame pFrame) {
			//If a request is received start processing the request
			//If a response is received, let the requestmanager handle
			//setting the response
			var objFame = new ControlFrame(pFrame.Data);
			if (objFame.FrameType == ControlFrameType.Request) {
				ActionManager.Process(pConnection, objFame);
			} else if (objFame.FrameType == ControlFrameType.Response) {
				RequestManager.Receive(pFrame);
			}
		}
	}
}