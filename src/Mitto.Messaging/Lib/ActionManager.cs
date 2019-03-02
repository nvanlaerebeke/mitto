using Mitto.IMessaging;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Mitto.Messaging {
	internal class ActionManager : IActionManager {
		private ConcurrentDictionary<string, IAction> Actions = new ConcurrentDictionary<string, IAction>();

		public void RunAction(IClient pClient, IRequestMessage pMessage, IAction pAction) {
			if (pAction == null) { return; } // -- nothing to do

			if (Actions.TryAdd(pMessage.ID, pAction)) {
				ThreadPool.QueueUserWorkItem(s => {
					switch (pMessage.Type) {
						case MessageType.Notification:
							try {
								((INotificationAction)pAction).Start();
							} catch (Exception) { /* ignore */ }
							break;
						case MessageType.Request:
							try {
								pClient.Transmit(
									((IRequestAction)pAction).Start()
								);
							} catch (Exception ex) {
								ResponseCode enmCode = (ex is MessagingException) ? ((MessagingException)ex).Code : ResponseCode.Error;
								var objResponse = MessagingFactory.Provider.GetResponseMessage(pMessage, enmCode);
								if (objResponse != null) {
									pClient.Transmit(objResponse);
								}
							}
							break;
					}
					Actions.TryRemove(pMessage.ID, out IAction objAction);
				});
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