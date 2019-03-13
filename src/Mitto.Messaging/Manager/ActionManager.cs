using Mitto.ILogging;
using Mitto.IMessaging;
using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Mitto.Messaging {
	internal class ActionManager : IActionManager {
		private ILog Log {
			get { return LogFactory.GetLogger(MethodBase.GetCurrentMethod().DeclaringType); }
		}
		private ConcurrentDictionary<string, IAction> Actions = new ConcurrentDictionary<string, IAction>();

		public void RunAction(IClient pClient, IRequestMessage pMessage, IAction pAction) {
			if (pAction == null) { // -- nothing to do
				Log.Error($"Unable to create action for {pMessage.Name} for {pClient.ID}");
				return;
			}
			Log.Info($"Starting action {pAction.GetType().Name} for {pClient.ID}");
			if (Actions.TryAdd(pMessage.ID, pAction)) {
				switch (pMessage.Type) {
					case MessageType.Notification:
						try {
							((INotificationAction)pAction).Start();
						} catch (Exception) { /* ignore */ }
						break;
					case MessageType.Request:
					case MessageType.Sub:
					case MessageType.UnSub:
						try {
							pClient.Transmit(
								(IResponseMessage)pAction.GetType().GetMethod("Start").Invoke(pAction, new object[0])
							);
						} catch (TargetInvocationException objTargetInvocationException) {
							var ex = objTargetInvocationException.InnerException;
							ResponseStatus objStatus = (ex is MessagingException) ? ((MessagingException)ex).Status : new ResponseStatus(ResponseState.Error);
							var objResponse = MessagingFactory.Provider.GetResponseMessage(pMessage, objStatus);
							if (objResponse != null) {
								pClient.Transmit(objResponse);
							}
						}
						break;
				}
				Actions.TryRemove(pMessage.ID, out IAction objAction);
			}
		}

		public MessageStatusType GetStatus(string pRequestID) {
			if (Actions.ContainsKey(pRequestID)) {
				return MessageStatusType.Busy;
			}
			return MessageStatusType.UnKnown;
		}
	}
}