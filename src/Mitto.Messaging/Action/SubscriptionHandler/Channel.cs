using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Mitto.IMessaging;
using Mitto.Messaging.Request;

namespace Mitto.Messaging.Action.SubscriptionHandler {
	public class Channel :
		ISubscriptionHandler<
			Mitto.Messaging.Subscribe.Channel,
			Mitto.Messaging.UnSubscribe.Channel,
			Mitto.Messaging.Request.ISendToChannel
		>, IChannel {
		private ConcurrentDictionary<string, List<IClient>> _dicSubscriptions = new ConcurrentDictionary<string, List<IClient>>();

		public bool Notify(IClient pSender, ISendToChannel pNotifyMessage) {
			try {
				if (_dicSubscriptions.ContainsKey(pNotifyMessage.ChannelName)) {
					foreach (var objClient in _dicSubscriptions[pNotifyMessage.ChannelName]) {
						if (!objClient.Equals(pSender)) {
							objClient.Transmit(
								new Messaging.Request.ReceiveOnChannel(
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

		public bool Sub(IClient pClient, Messaging.Subscribe.Channel pMessage) {
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

		public bool UnSub(IClient pClient, Messaging.UnSubscribe.Channel pMessage) {
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