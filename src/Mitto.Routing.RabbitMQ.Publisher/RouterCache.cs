using Mitto.IConnection;
using Mitto.IRouting;
using System.Collections.Concurrent;

namespace Mitto.Routing.RabbitMQ.Publisher {

    /// <summary>
    /// ToDo: save all IRouters in the same collection
    /// </summary>
	internal class RouterCache {
        private readonly ConcurrentDictionary<string, IRouter> RouterPerConnectionID = new ConcurrentDictionary<string, IRouter>();
        private readonly ConcurrentDictionary<string, IRouter> RouterByID = new ConcurrentDictionary<string, IRouter>();
        private readonly ConcurrentDictionary<string, IRouter> ConsumerRouters = new ConcurrentDictionary<string, IRouter>();

        private readonly RequestManager RequestManager;
        private readonly SenderQueue MainQueue;

        internal RouterCache(SenderQueue pMainQueue, RequestManager pMessageManager) {
            MainQueue = pMainQueue;
            RequestManager = pMessageManager;
        }

        internal IRouter GetByConsumerID(string pConsumerID) {
            if (ConsumerRouters.ContainsKey(pConsumerID)) {
                if (ConsumerRouters.TryGetValue(pConsumerID, out IRouter objRouter)) {
                    return objRouter;
                } else {
                    //ToDo: error logging
                    System.Threading.Thread.Sleep(5); // wait a bit and try again
                }
            } else {
                var objRouter = new ConsumerRouter(new SenderQueue(QueueType.Consumer, pConsumerID, false), RequestManager);
                objRouter.Disconnected += ObjRouter_Disconnected;
                if (!ConsumerRouters.TryAdd(pConsumerID, objRouter)) {
                    //ToDo: error logging
                    System.Threading.Thread.Sleep(5); // wait a bit and try again
                }
            }
            return GetByConsumerID(pConsumerID);
        }

        internal IRouter GetByConnection(IClientConnection pConnection) {
            IRouter objRouter = null;
            if (RouterPerConnectionID.ContainsKey(pConnection.ID)) {
                if (RouterPerConnectionID.TryGetValue(pConnection.ID, out objRouter)) {
                    return objRouter;
                } else {
                    //ToDo: logging
                }
            }

            var obj = new Router(MainQueue, RequestManager, pConnection);
            obj.Disconnected += Router_Disconnected;
            if (!RouterPerConnectionID.TryAdd(pConnection.ID, obj)) {
                //ToDo: logging
            }
            if (!RouterByID.TryAdd(obj.ConnectionID, obj)) {
                //ToDo: logging
            }
            return obj;
        }

        internal bool ContainConnection(string pClientID) {
            return RouterPerConnectionID.ContainsKey(pClientID);
        }

        internal IRouter GetByRouterID(string pRouterID) {
            if (RouterByID.ContainsKey(pRouterID)) {
                if (RouterByID.TryGetValue(pRouterID, out IRouter objRouter)) {
                    return objRouter;
                }
            }
            return null;
        }

        private void Router_Disconnected(object sender, Router e) {
            e.Disconnected -= Router_Disconnected;
            if (RouterPerConnectionID.ContainsKey(e.ConnectionID)) {
                if (!RouterPerConnectionID.TryRemove(e.ConnectionID, out IRouter objRouter1)) {
                    //ToDo: Logging
                }
                if (!RouterByID.TryRemove(e.ConnectionID, out IRouter objRouter2)) {
                    //ToDo: Logging
                }
            }
        }

        private void ObjRouter_Disconnected(object sender, IRouter e) {
            ConsumerRouters.TryRemove(e.ConnectionID, out _);
        }
    }
}