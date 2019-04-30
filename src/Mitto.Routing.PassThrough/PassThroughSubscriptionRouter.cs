using Mitto.IMessaging;
using Mitto.IRouting;
using Mitto.Logging;
using Mitto.Subscription.Messaging;
using System;
using System.Collections.Concurrent;

namespace Mitto.Routing.PassThrough {

    internal class PassThroughSubscriptionRouter<T> : IRouter {
        private readonly ConcurrentDictionary<string, IMessage> Requests = new ConcurrentDictionary<string, IMessage>();
        private readonly ILog Log = LoggingFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IClient Client;

        public event EventHandler<IRouter> Disconnected;

        public string SourceID => Client.ID;
        public string DestinationID => Client.ID;

        public string ConnectionID => Client.ID;

        public PassThroughSubscriptionRouter(IClient pClient) {
            Client = pClient;
            Client.Disconnected += Router_Disconnected;
        }

        public void Transmit(byte[] pData) {
            RoutingFrame objRoutingFrame = new RoutingFrame(pData);
            IMessage objMessage = MessagingFactory.Provider.GetMessage(objRoutingFrame.Data);

            if (objMessage == null) {
                Log.Error($"Unable to create message for request {objRoutingFrame.RequestID}");
                return;
            }

            T objHandler = MessagingFactory.Provider.GetSubscriptionHandler<T>();
            if (objHandler == null) {
                Log.Error($"Unable to get subscription handler for {typeof(T)}");
                return;
            }

            if (!Requests.TryAdd(objRoutingFrame.RequestID, objMessage)) {
                Log.Error($"Unable to add Request {objMessage.Name}({objRoutingFrame.RequestID}) to list, unable to track");
            }
            bool blnResult = false;
            if (objMessage is SubMessage) {
                blnResult = (bool)objHandler.GetType().GetMethod("Sub").Invoke(objHandler, new object[] { Client, objMessage });
            } else if (objMessage is UnSubMessage) {
                blnResult = (bool)objHandler.GetType().GetMethod("UnSub").Invoke(objHandler, new object[] { Client, objMessage });
            } else if (objMessage is IRequestMessage) {
                blnResult = (bool)objHandler.GetType().GetMethod("Notify").Invoke(objHandler, new object[] { Client, objMessage });
            } else {
                Log.Error($"Unsupported message type for subscription router of type {typeof(T)}");
            }

            IResponseMessage objResponse = MessagingFactory.Provider.GetResponseMessage(
                objMessage as IRequestMessage,
                new ResponseStatus(
                    (blnResult) ? ResponseState.Success : ResponseState.Error
                )
            );
            Client.Transmit(objResponse);

            /*Client.Receive(
                new RoutingFrame(
                    RoutingFrameType.Messaging,
                    MessageType.Response,
                    objMessage.ID,
                    ConnectionID,
                    ConnectionID,
                    new Frame(
                        MessageType.Response,
                        objMessage.ID,
                        objResponse.Name,
                        MessagingFactory.Converter.GetByteArray(objResponse)
                    ).GetByteArray()
                ).GetBytes()
            );*/

            if (!Requests.TryRemove(objRoutingFrame.RequestID, out _)) {
                Log.Error($"Unable to remove {objMessage.Name}({objMessage.ID}) from tracking list, leaking memory");
            }
        }

        public void Receive(byte[] pData) {
            MessagingFactory.Processor.Process(Client.Router, pData);
        }

        public void Close() {
            Client.Disconnected -= Router_Disconnected;
        }

        public bool IsAlive(string pRequestID) {
            return (Requests.ContainsKey(pRequestID));
        }

        private void Router_Disconnected(object sender, IClient e) {
            Client.Disconnected -= Router_Disconnected;
            Disconnected?.Invoke(sender, this);
        }
    }
}