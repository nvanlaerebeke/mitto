﻿using Mitto.IMessaging;

namespace Mitto.Messaging.Action.Notification
{
    public class LogStatusNotificationAction: NotificationAction<Messaging.Notification.LogStatusNotification> {
        public LogStatusNotificationAction(IClient pClient, Messaging.Notification.LogStatusNotification pMessage) : base(pClient, pMessage) { }

        public override void Start() {
            //only allow this by servers

            /*lock(Messaging.SyncServer._lstClients) {
                Log.Info("Connected client count: " + Messaging.SyncServer._lstClients.Count);
                foreach(var objClient in Messaging.SyncServer._lstClients) {
                    Log.Info("ID: " + objClient.ID + "IP: " + objClient.ClientIP + " Status: " + objClient.Status);
                }
            }

            lock(Messaging.SyncServer._lstServers) {
                Log.Info("Connected server count: " + Messaging.SyncServer._lstServers.Count);
                foreach(var objClient in Messaging.SyncServer._lstServers) {
                    Log.Info("ID: " + objClient.ID + "IP: " + objClient.ClientIP + " Status: " + objClient.Status);
                }
            }

            lock(SubscriptionHandler._dicHandlers) {
                Log.Info("Event Handlers: " + SubscriptionHandler._dicHandlers.Count);
                foreach(var objHandler in SubscriptionHandler._dicHandlers) {
                    Log.Info("Type: " + objHandler.Key);
                    Log.Info("Handler Info:");
                    foreach(var info in objHandler.Value.Info()) {
                        Log.Info(info);
                    }
                }
            }*/
        }
	}
}