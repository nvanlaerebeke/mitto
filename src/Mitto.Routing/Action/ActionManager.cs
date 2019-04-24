using Mitto.IRouting;
using Mitto.Logging;
using Mitto.Routing.Response;
using System;
using System.Collections.Concurrent;

namespace Mitto.Routing.Action {

    internal class ActionManager {
        private readonly ILog Log = LoggingFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ConcurrentDictionary<string, IControlAction> Actions = new ConcurrentDictionary<string, IControlAction>();

        public ActionManager() {
        }

        public void Process(IRouter pConnection, ControlFrame pFrame) {
            var obj = ControlFactory.Provider.GetAction(pConnection, pFrame);
            if (obj == null) {
                Log.Error($"Failed to create message {pFrame.MessageName}");
                return;
            }
            if (!Actions.TryAdd(pFrame.RequestID, obj)) {
                Log.Error($"Failed adding {pFrame.MessageName}({pFrame.RequestID}) action to collection, ignoring action");
            } else {
                try {
                    var objResponse = (IControlResponse)obj.GetType().GetMethod("Start").Invoke(obj, new object[0]);
                    Log.Debug($"Response gotten for RequestID {pFrame.RequestID}");
                    ControlFrame objControlFrame = objResponse.GetFrame();
                    if (objResponse != null) {
                        pConnection.Transmit(
                            new RoutingFrame(
                                RoutingFrameType.Control,
                                MessageType.Response,
                                pFrame.RequestID,
                                pConnection.ConnectionID,
                                "",
                                objResponse.GetFrame().GetBytes()
                            ).GetBytes()
                        );
                    }
                } catch (Exception ex) {
                    Log.Error(ex);
                }
            }
        }
    }
}