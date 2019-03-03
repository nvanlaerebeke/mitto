using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Mitto.IMessaging;
using Mitto.Messaging.Request;

namespace Mitto.Messaging.Action.SubscriptionHandler {
	public class ChannelSubscriptionHandler :
		ISubscriptionHandler<
			Mitto.Messaging.Subscribe.ChannelSubscribe,
			Mitto.Messaging.UnSubscribe.ChannelUnSubscribe,
			Mitto.Messaging.Request.ISendToChannelRequest
		>, IChannelSubscriptionHandler {
		private ConcurrentDictionary<string, List<IClient>> _dicSubscriptions = new ConcurrentDictionary<string, List<IClient>>();

		public bool Notify(IClient pSender, ISendToChannelRequest pNotifyMessage) {
			try {
				if (_dicSubscriptions.ContainsKey(pNotifyMessage.ChannelName)) {
					foreach (var objClient in _dicSubscriptions[pNotifyMessage.ChannelName]) {
						if (!objClient.Equals(pSender)) {
							objClient.Transmit(
								new Messaging.Request.ReceiveOnChannelRequest(
									pNotifyMessage.ChannelName, 
									pNotifyMessage.Message
								)
							);
						}
					}
				}
				return true;
			} catch (Exception) {
				return false;
			}
		}

		public bool Sub(IClient pClient, Messaging.Subscribe.ChannelSubscribe pMessage) {
			try {
				if (!_dicSubscriptions.ContainsKey(pMessage.ChannelName)) {
					_dicSubscriptions.TryAdd(pMessage.ChannelName, new List<IClient>());
				}
				lock (_dicSubscriptions[pMessage.ChannelName]) {
					_dicSubscriptions[pMessage.ChannelName].Add(pClient);
				}
				return true;
			} catch (Exception) {
				return false;
			}
		}

		public bool UnSub(IClient pClient, Messaging.UnSubscribe.ChannelUnSubscribe pMessage) {
			try {
				if (!_dicSubscriptions.ContainsKey(pMessage.ChannelName)) { return true; } // -- nothing to do 
				lock (_dicSubscriptions[pMessage.ChannelName]) {
					_dicSubscriptions[pMessage.ChannelName].RemoveAll(c => c.ID.Equals(pClient.ID));
					return true;
				}
			} catch (Exception) {
				return false;
			}
		}
	}
}