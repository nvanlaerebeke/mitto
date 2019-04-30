using Mitto.IMessaging;
using Mitto.Logging;
using Mitto.Messaging.Response;
using Mitto.Subscription.Messaging.Request;
using Mitto.Subscription.Messaging.Subscribe;
using Mitto.Subscription.Messaging.UnSubscribe;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Mitto.Subscription.Messaging.Handlers {

    public class ChannelSubscriptionHandler :
        ISubscriptionHandler<
            ChannelSubscribe,
            ChannelUnSubscribe,
            SendToChannelRequest
        > {
        private readonly ILog Log = LoggingFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ConcurrentDictionary<string, List<IClient>> _dicSubscriptions = new ConcurrentDictionary<string, List<IClient>>();

        public bool NotifyAll(SendToChannelRequest pNotifyMessage) {
            return Notify(null, pNotifyMessage);
        }

        public bool Notify(IClient pSender, SendToChannelRequest pNotifyMessage) {
            List<IClient> lstClients = new List<IClient>();
            try {
                if (_dicSubscriptions.ContainsKey(pNotifyMessage.ChannelName)) {
                    lstClients = _dicSubscriptions[pNotifyMessage.ChannelName];
                }
            } catch (Exception ex) {
                Log.Error($"Failed getting clients to forward {pNotifyMessage.ChannelName} message to");
                Log.Error(ex);
                return false;
            }

            try {
                ReceiveOnChannelRequest objMessage = new ReceiveOnChannelRequest(pNotifyMessage.ChannelName, pNotifyMessage.Message);

                foreach (IClient objClient in lstClients) {
                    if (pSender != null && objClient.ID.Equals(pSender.ID)) { continue; } // -- skip sender
                    objClient.Request<ACKResponse>(objMessage, (r) => {
                        if (r.Status.State == ResponseState.Success) {
                            Log.Info($"Notification successfully sent to {objClient.ID}");
                        } else {
                            Log.Error($"Failed to deliver notification for {objClient.ID}");
                        }
                    });
                }
                return true;
            } catch (Exception ex) {
                Log.Error(ex);
                return false;
            }
        }

        public bool Sub(IClient pClient, ChannelSubscribe pMessage) {
            try {
                if (!_dicSubscriptions.ContainsKey(pMessage.ChannelName)) {
                    _dicSubscriptions.TryAdd(pMessage.ChannelName, new List<IClient>());
                }
                lock (_dicSubscriptions[pMessage.ChannelName]) {
                    if (!_dicSubscriptions[pMessage.ChannelName].Any(c => c.ID.Equals(pClient.ID))) {
                        _dicSubscriptions[pMessage.ChannelName].Add(pClient);
                    }
                }
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public bool UnSub(IClient pClient, ChannelUnSubscribe pMessage) {
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