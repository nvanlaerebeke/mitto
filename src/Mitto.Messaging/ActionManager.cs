using Mitto.IMessaging;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Mitto.Messaging {
	internal class ActionManager : IActionManager {
		private ConcurrentDictionary<string, IAction> Actions = new ConcurrentDictionary<string, IAction>();

		public void RunAction(IQueue.IQueue pClient, IMessage pMessage, IAction pAction) {
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
									MessagingFactory.Provider.GetByteArray(
										((IRequestAction)pAction).Start()
									)
								);
							} catch (Exception ex) {
								ResponseCode enmCode = (ex is MessagingException) ? ((MessagingException)ex).Code : ResponseCode.Error;
								var objResponse = MessagingFactory.Provider.GetResponseMessage(pMessage, enmCode);
								if (objResponse != null) {
									pClient.Transmit(MessagingFactory.Provider.GetByteArray(objResponse));
								}
							}
							break;
					}
					Actions.TryRemove(pMessage.ID, out IAction objAction);
				});
			}
		}

		public bool IsBusy(string pID) {
			return Actions.ContainsKey(pID);
		}
	}
}