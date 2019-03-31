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

		/// <summary>
		/// ToDo: pass IRequest instead of IRouter + the Frame
		/// </summary>
		/// <param name="pConnection"></param>
		/// <param name="pFrame"></param>
		public void Process(IRouter pConnection, RoutingFrame pFrame) {
			//If a request is received start processing the request
			//If a response is received, let the requestmanager handle
			//setting the response
			var objFame = new ControlFrame(pFrame.Data);
			if (objFame.FrameType == MessageType.Request) {
				ActionManager.Process(pConnection, objFame);
			} else if (objFame.FrameType == MessageType.Response) {
				RequestManager.Receive(pConnection, pFrame);
			}
		}
	}
}