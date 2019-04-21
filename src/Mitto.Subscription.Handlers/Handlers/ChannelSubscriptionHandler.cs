using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Mitto.IMessaging;
using Mitto.ILogging;
using Mitto.IRouting;
using Mitto.Messaging.Response;
using Mitto.Subscription.Messaging.Request;
using Mitto.Subscription.Messaging.UnSubscribe;
using Mitto.Subscription.Messaging.Subscribe;
using System.Linq;

namespace Mitto.Subscription.Messaging.Handlers {

    public class ChannelSubscriptionHandler :
        ISubscriptionHandler<
            ChannelSubscribe,
            ChannelUnSubscribe,
            SendToChannelRequest
        > {

        private ILog Log {
            get {
                return LogFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

        private ConcurrentDictionary<string, List<IRouter>> _dicSubscriptions = new ConcurrentDictionary<string, List<IRouter>>();

        public bool Notify(IRouter pSender, SendToChannelRequest pNotifyMessage) {
            var lstRouters = new List<IRouter>();
            try {
                if (_dicSubscriptions.ContainsKey(pNotifyMessage.ChannelName)) {
                    lstRouters = _dicSubscriptions[pNotifyMessage.ChannelName];
                }
            } catch (Exception ex) {
                Log.Error($"Failed getting clients to forward {pNotifyMessage.ChannelName} message to");
                Log.Error(ex);
                return false;
            }

            try {
                var objMessage = new ReceiveOnChannelRequest(pNotifyMessage.ChannelName, pNotifyMessage.Message);

                foreach (var objRouter in lstRouters) {
                    if (objRouter.ConnectionID.Equals(pSender.ConnectionID)) { continue; } // -- skip sender
                    MessagingFactory.Processor.Request<ACKResponse>(
                        objRouter,
                        objMessage,
                        (r) => {
                            if (r.Status.State != ResponseState.Success) {
                                Log.Error($"Failed to forward {pNotifyMessage.ChannelName} message to {objRouter.ConnectionID}");
                            }
                        }
                    );
                }
                return true;
            } catch (Exception ex) {
                Log.Error(ex);
                return false;
            }
        }

        public bool Sub(IRouter pClient, ChannelSubscribe pMessage) {
            try {
                if (!_dicSubscriptions.ContainsKey(pMessage.ChannelName)) {
                    _dicSubscriptions.TryAdd(pMessage.ChannelName, new List<IRouter>());
                }
                lock (_dicSubscriptions[pMessage.ChannelName]) {
                    if (!_dicSubscriptions[pMessage.ChannelName].Any(c => c.ConnectionID.Equals(pClient.ConnectionID))) {
                        _dicSubscriptions[pMessage.ChannelName].Add(pClient);
                    }
                }
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public bool UnSub(IRouter pClient, ChannelUnSubscribe pMessage) {
            try {
                if (!_dicSubscriptions.ContainsKey(pMessage.ChannelName)) { return true; } // -- nothing to do
                lock (_dicSubscriptions[pMessage.ChannelName]) {
                    _dicSubscriptions[pMessage.ChannelName].RemoveAll(c => c.ConnectionID.Equals(pClient.ConnectionID));
                    return true;
                }
            } catch (Exception) {
                return false;
            }
        }
    }
}