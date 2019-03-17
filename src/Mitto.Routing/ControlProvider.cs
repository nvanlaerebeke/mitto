using Mitto.IRouting;
using Mitto.Routing.Action;
using Mitto.Routing.Request;
using Mitto.Routing.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mitto.Routing.Tests")]
namespace Mitto.Routing {
	/// <summary>
	/// ToDo: put all the types into one dictionary 
	/// </summary>
	public class ControlProvider {
		internal List<ActionInfo> Actions = new List<ActionInfo>();
		internal Dictionary<string, Type> Requests = new Dictionary<string, Type>();
		internal Dictionary<string, Type> Responses = new Dictionary<string, Type>();

		public ControlProvider() {
			Load();
		}

		private void Load() {
			List<Type> lstSupportedTypes = new List<Type>() {
				typeof(IControlAction),
				typeof(IControlRequest),
				typeof(IControlResponse)
			};
			foreach (var ass in AppDomain.CurrentDomain.GetAssemblies()) {
				try {
					(
						from t in ass.GetTypes()
						where
							t.IsClass &&
							!t.IsAbstract &&
							t.GetInterfaces().Any(i => lstSupportedTypes.Contains(i))
						select t
					).ToList().ForEach(t => {
						if (t.GetInterfaces().Contains(typeof(IControlAction))) {
							var tmpType = t;
							while(
								tmpType != null &&
								!tmpType.FullName.Contains("Mitto.Routing.Action.BaseControlAction")
							) { tmpType = t.BaseType; }

							if (tmpType != null) {
								Type objRequestType = (tmpType.GenericTypeArguments.Length > 0) ? tmpType.GenericTypeArguments[0] : null;
								Type objResponseType = (tmpType.GenericTypeArguments.Length > 1) ? tmpType.GenericTypeArguments[1] : null;
								if (objRequestType != null && objResponseType != null) {
									Actions.Add(new ActionInfo(objRequestType, objResponseType, t));
								}
							}
						} else if (t.GetInterfaces().Contains(typeof(IControlRequest))) {
							Requests.Add(t.Name, t);
						} else if (t.GetInterfaces().Contains(typeof(IControlResponse))) {
							Responses.Add(t.Name, t);
						}
					});
				} catch (Exception) { }
			}
		}

		public IControlAction GetAction(IRouter pConnection, ControlFrame pFrame) {
			var objInfo = Actions.Where(a => a.RequestType.Name == pFrame.MessageName).FirstOrDefault();
			if(objInfo != null) {
				return (IControlAction)Activator.CreateInstance(objInfo.ActionType, pConnection, GetMessage(pFrame));
			}
			return null;
		}

		public ControlMessage GetMessage(ControlFrame pFrame) {
			ControlMessage objMessage = null;
			if(pFrame.FrameType == ControlFrameType.Request) {
				if(Requests.ContainsKey(pFrame.MessageName)) {
					objMessage = (ControlMessage)Activator.CreateInstance(Requests[pFrame.MessageName], pFrame);
				}
			} else {
				if (Responses.ContainsKey(pFrame.MessageName)) {
					objMessage = (ControlMessage)Activator.CreateInstance(Responses[pFrame.MessageName], pFrame);
				}
			}
			return objMessage;
		}

		internal class ActionInfo {
			public readonly Type RequestType;
			public readonly Type ResponseType;
			public readonly Type ActionType;
			public ActionInfo(Type pRequestType, Type pResponseType, Type pActionType) {
				RequestType = pRequestType;
				ResponseType = pResponseType;
				ActionType = pActionType;
			}
		}
	}
}