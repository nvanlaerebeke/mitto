using Mitto.IRouting;
using Mitto.Routing.Response;
using System;
using System.Collections.Concurrent;

namespace Mitto.Routing.Action {
	internal class ActionManager {
		private ConcurrentDictionary<string, IControlAction> Actions = new ConcurrentDictionary<string, IControlAction>();
		public ActionManager() { }

		public void Process(IRouter pConnection, ControlFrame pFrame) {
			var obj = ControlFactory.Provider.GetAction(pConnection, pFrame);
			if(!Actions.TryAdd(pFrame.RequestID, obj)) {
				//ToDo: error handling/logging
			} else {
				try {
					var objResponse = (IControlResponse)obj.GetType().GetMethod("Start").Invoke(obj, new object[0]);
					if (objResponse != null) {
						pConnection.Transmit(
							new RoutingFrame(
								RoutingFrameType.Control,
								pFrame.RequestID,
								pConnection.ConnectionID,
								"",
								objResponse.GetFrame().GetBytes()
							).GetBytes()
						);
					}
				} catch (Exception) {
					//ToDo: error handling
				}
			}
		}
	}
}