using Mitto.IMessaging;
using Mitto.IQueue;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Mitto.Messaging {
	/// <summary>
	/// ToDo: make the MessageProvider return the Action object so 
	/// that we can mock it don't need all this Reflection stuff
	/// </summary>
	internal class RequestManager : IRequestManager {
		private ConcurrentDictionary<string, MessageRequest> _dicRequests = new ConcurrentDictionary<string, MessageRequest>();

		public void Request<R>(MessageClient pClient, IMessage pMessage, Action<R> pAction) where R : IResponseMessage {
			lock (_dicRequests) {
				var obj = new MessageRequest();
				obj.Request<R>(pClient, pMessage, pAction);
				_dicRequests.TryAdd(pMessage.ID, obj);
			}
		}

		public void SetResponse(IResponseMessage pMessage) {
			lock (_dicRequests) {
				if (_dicRequests.ContainsKey(pMessage.ID)) {
					_dicRequests[pMessage.ID].SetResponse(pMessage);
				}
			}
		}

		public void Process(IQueue.IQueue pClient, IMessage pMessage) {
			//Message is a response on an action, so no action needs to run
			//set the response and return
			if (pMessage.Type == MessageType.Response) {
				SetResponse(pMessage as IResponseMessage);
				return;
			}

			var objActionType = MessagingFactory.Provider.GetActionType(pMessage);
			if (objActionType == null) { return; } // -- nothing to do

			switch (pMessage.Type) {
				case MessageType.Notification:
					try {
						objActionType.GetMethod("Start").Invoke(Activator.CreateInstance(objActionType, pClient, pMessage), new object[] { });
					} catch (Exception) { /* ignore */ }
					break;
				case MessageType.Request:
				case MessageType.Subscribe:
				case MessageType.UnSubscribe:
				case MessageType.Event:
					try {
						pClient.Transmit(
							MessagingFactory.Converter.GetByteArray(
								(ResponseMessage)objActionType.GetMethod("Start").Invoke(
									Activator.CreateInstance(objActionType, pClient, pMessage), new object[] { }
								)
							)
						);
					} catch (Exception ex) {
						ResponseCode enmCode = (ex is MessagingException) ? ((MessagingException)ex).Code : ResponseCode.Error;
						var objResponse = MessagingFactory.Converter.GetResponseMessage(pMessage, enmCode);
						if (objResponse != null) {
							pClient.Transmit(MessagingFactory.Converter.GetByteArray(objResponse));
						}
					}
					break;
			}
		}
	}
}