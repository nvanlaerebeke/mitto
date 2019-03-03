using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Mitto.IMessaging;
using ChatSample.Messaging.Request;
using ChatSample.Messaging.UnSubscribe;
using ChatSample.Messaging.Subscribe;

namespace ChatSampleServer.Action.SubscriptionHandler {
	public class ChatSubscriptionHandler :
		ISubscriptionHandler<
			ChatSubscribe,
			ChatUnSubscribe,
			SendMessageRequest
		> {
		private ConcurrentDictionary<string, List<IClient>> _dicSubscriptions = new ConcurrentDictionary<string, List<IClient>>();

		public bool Sub(IClient pClient, ChatSubscribe pMessage) {
			try {
				if (!_dicSubscriptions.ContainsKey(pMessage.Channel)) {
					_dicSubscriptions.TryAdd(pMessage.Channel, new List<IClient>());
				}
				lock (_dicSubscriptions[pMessage.Channel]) {
					_dicSubscriptions[pMessage.Channel].Add(pClient);
				}
				return true;
			} catch (Exception) {
				return false;
			}
		}

		public bool UnSub(IClient pClient, ChatSample.Messaging.UnSubscribe.ChatUnSubscribe pMessage) {
			try {
				if (!_dicSubscriptions.ContainsKey(pMessage.Channel)) { return true; } // -- nothing to do 
				lock (_dicSubscriptions[pMessage.Channel]) {
					return _dicSubscriptions[pMessage.Channel].Remove(pClient);
				}
			} catch (Exception) {
				return false;
			}
		}

		public bool Notify(IClient pSender, ChatSample.Messaging.Request.SendMessageRequest pNotifyMessage) {
			try {
				if (_dicSubscriptions.ContainsKey(pNotifyMessage.Channel)) {
					foreach (var objClient in _dicSubscriptions[pNotifyMessage.Channel]) {
						if (!objClient.Equals(pSender)) {
							objClient.Transmit(new ReceiveMessageRequest(pNotifyMessage.Channel, pNotifyMessage.Message));
						}
					}
				}
				return true;
			} catch (Exception) {
				return false;
			}
		}
	}
}