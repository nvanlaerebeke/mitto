using Mitto.IRouting;
using Mitto.Logging;
using Mitto.Routing.Action;
using System;

namespace Mitto.Routing {

    public class ControlProcessor {
        private readonly ILog Log = LoggingFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
            //If a response is received, let the request manager handle
            //setting the response
            ControlFrame objFame;
            try {
                objFame = new ControlFrame(pFrame.Data);
            } catch {
                Log.Error("Invalid data received, ignoring...");
                return;
            }

            try {
                if (objFame.FrameType == MessageType.Request) {
                    ActionManager.Process(pConnection, objFame);
                } else if (objFame.FrameType == MessageType.Response) {
                    RequestManager.Receive(pConnection, pFrame);
                }
            } catch (Exception ex) {
                Log.Error($"Unable to process control frame: {ex.Message}, ignoring...");
                return;
            }
        }
    }
}