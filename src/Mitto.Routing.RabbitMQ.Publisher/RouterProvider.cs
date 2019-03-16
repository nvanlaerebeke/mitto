using Mitto.IConnection;
using Mitto.IRouting;
using System.Collections.Concurrent;

/// <summary>
/// ToDo: Create a router for each connection, but do not create a reader queue for
/// each client seperatly, both the main queue (Mitto.Main) and the publisher Queue are 
/// unique per process - so no need to doublicate it for each connection
/// </summary>
namespace Mitto.Routing.RabbitMQ.Publisher {
	public class RouterProvider : IRouterProvider {
		/// <summary>
		/// Unique identifier for this Provider
		/// </summary>
		public static string ID { get; } = $"Mitto.Publisher.{System.Guid.NewGuid().ToString()}";

		private readonly SenderQueue MainQueue;
		private readonly ReaderQueue PublisherQueue;
		private readonly RequestManager RequestManager;
		private readonly ConcurrentDictionary<string, Router> Routers = new ConcurrentDictionary<string, Router>();
		private readonly QueueProvider QueueProvider;

		public RouterProvider(RabbitMQParams pParams) {
			MainQueue = new SenderQueue("Mitto.Main");
			PublisherQueue = new ReaderQueue(ID);
			RequestManager = new RequestManager();
			QueueProvider = new QueueProvider(pParams);

			PublisherQueue.Rx += PublisherQueue_Rx;
		}

		private void PublisherQueue_Rx(object sender, Frame e) {
			if (Routers.TryGetValue(e.ConnectionID, out Router objRouter)) {
				objRouter.Process(QueueProvider.GetSenderQueue(e.QueueID), e);
			} else {
				//ToDo: Logging
			}
		}


		/// <summary>
		/// Creates a Router object that provides communication between the client
		/// and this publisher
		/// </summary>
		/// <param name="pConnection"></param>
		/// <returns></returns>
		public IRouter Create(IClientConnection pConnection) {
			//Remove if already present
			if(Routers.ContainsKey(pConnection.ID)) {
				if(Routers.TryRemove(pConnection.ID, out Router objRouter)) {
					objRouter.Close();
				}
			}

			var obj = new Router(MainQueue, RequestManager, pConnection);
			obj.Disconnected += Router_Disconnected;
			if(!Routers.TryAdd(pConnection.ID, obj)) {
				//ToDo: logging
			}
			return obj;
		}

		private void Router_Disconnected(object sender, Router e) {
			e.Disconnected -= Router_Disconnected;
			if (Routers.ContainsKey(e.ConnectionID)) {
				if (!Routers.TryRemove(e.ConnectionID, out Router objRouter)) {
					//ToDo: Logging
				}
			}
		}
	}
}