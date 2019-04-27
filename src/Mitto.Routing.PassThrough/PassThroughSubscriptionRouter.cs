﻿using Mitto.IMessaging;
using Mitto.IRouting;
using Mitto.Logging;
using Mitto.Subscription.Messaging;
using System;
using System.Collections.Concurrent;

namespace Mitto.Routing.PassThrough {

    internal class PassThroughSubscriptionRouter<T> : IRouter {
        private ConcurrentDictionary<string, IMessage> Requests = new ConcurrentDictionary<string, IMessage>();
        private readonly ILog Log = LoggingFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IRouter Router;
        public string SourceID { get { return Router.ConnectionID; } }
        public string DestinationID { get { return Router.ConnectionID; } }

        public string ConnectionID { get { return Router.ConnectionID; } } 

        public PassThroughSubscriptionRouter(IRouter pRouter) {
            Router = pRouter;
        }

        public void Transmit(byte[] pData) {
            var objRoutingFrame = new RoutingFrame(pData);
            var objMessage = MessagingFactory.Provider.GetMessage(objRoutingFrame.Data);

            if (objMessage == null) {
                Log.Error($"Unable to create message for request {objRoutingFrame.RequestID}");
                return;
            }

            var objHandler = MessagingFactory.Provider.GetSubscriptionHandler<T>();
            if(objHandler == null) {
                Log.Error($"Unable to get subscription handler for {typeof(T)}");
                return;
            }

            if(!Requests.TryAdd(objRoutingFrame.RequestID, objMessage)) {
                Log.Error($"Unable to add Request {objMessage.Name}({objRoutingFrame.RequestID}) to list, unable to track");
            }
            var blnResult = false;
            if (objMessage is SubMessage) {
                blnResult = (bool)objHandler.GetType().GetMethod("Sub").Invoke(objHandler, new object[] { Router, objMessage });
            } else if (objMessage is UnSubMessage) {
                blnResult = (bool)objHandler.GetType().GetMethod("UnSub").Invoke(objHandler, new object[] { Router, objMessage });
            } else if (objMessage is IRequestMessage) {
                blnResult = (bool)objHandler.GetType().GetMethod("Notify").Invoke(objHandler, new object[] { Router, objMessage });
            } else {
                Log.Error($"Unsupport message type for subscription router of type {typeof(T)}");
            }

            var objResponse = MessagingFactory.Provider.GetResponseMessage(
                objMessage as IRequestMessage,
                new ResponseStatus(
                    (blnResult) ? ResponseState.Success : ResponseState.Error
                )
            );

            Receive(
                new RoutingFrame(
                    RoutingFrameType.Messaging, 
                    MessageType.Response,
                    objMessage.ID,
                    this.ConnectionID,
                    this.ConnectionID,
                    new Frame(
                        MessageType.Response,
                        objMessage.ID,
                        objResponse.Name,
                        MessagingFactory.Converter.GetByteArray(objResponse)
                    ).GetByteArray()
                ).GetBytes()
            );

            /*Receive(
                MessagingFactory.Converter.GetByteArray(
                    MessagingFactory.Provider.GetResponseMessage(
                        objMessage as IRequestMessage,
                        new ResponseStatus(
                            (blnResult) ? ResponseState.Success : ResponseState.Error
                        )
                    )
                )
            );*/

            if(!Requests.TryRemove(objRoutingFrame.RequestID, out _)) {
                Log.Error($"Unable to remove {objMessage.Name}({objMessage.ID}) from tracking list, leaking memory");
            }
        }

        public void Receive(byte[] pData) {
            Router.Transmit(pData);
        }

        public void Close() {
            //nothing to do - everything will be garbage collected automatically
        }

        public bool IsAlive(string pRequestID) {
            return (Requests.ContainsKey(pRequestID));
        }
    }
}