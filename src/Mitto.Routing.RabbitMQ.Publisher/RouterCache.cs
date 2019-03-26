using Mitto.IConnection;
using Mitto.IRouting;
using System.Collections.Concurrent;

namespace Mitto.Routing.RabbitMQ.Publisher {

	internal class RouterCache {
		private readonly ConcurrentDictionary<string, IRouter> RouterPerConnectionID = new ConcurrentDictionary<string, IRouter>();
		private readonly ConcurrentDictionary<string, IRouter> RouterByID = new ConcurrentDictionary<string, IRouter>();
		private readonly ConcurrentDictionary<string, IRouter> ConsumerRouters = new ConcurrentDictionary<string, IRouter>();
		private readonly RequestManager RequestManager;
		private readonly SenderQueue MainQueue;

		public RouterCache(SenderQueue pMainQueue, RequestManager pMessageManager) {
			MainQueue = pMainQueue;
			RequestManager = pMessageManager;
		}

		public IRouter GetByConsumerID(string pConsumerID) {
			if (ConsumerRouters.ContainsKey(pConsumerID)) {
				if (ConsumerRouters.TryGetValue(pConsumerID, out IRouter objRouter)) {
					return objRouter;
				} else {
					//new ConsumerRouter(Publisher, ConsumerQu);
					return null;
				}
			}
			return null;
		}

		public IRouter GetByConnection(IClientConnection pConnection) {
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

		public IRouter GetByRouterID(string pRouterID) {
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
	}
}