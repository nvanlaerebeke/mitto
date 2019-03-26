using Mitto.IConnection;
using Mitto.IRouting;

namespace Mitto.Routing.RabbitMQ.Publisher {

	public class RouterProvider : IRouterProvider {

		/// <summary>
		/// Unique identifier for this Provider
		/// </summary>
		public static string ID { get; } = $"Mitto.Publisher.{System.Guid.NewGuid().ToString()}";

		private readonly ReaderQueue PublisherQueue;
		private readonly RouterCache RouterCache;

		public RouterProvider(RabbitMQParams pParams) {
			RouterCache = new RouterCache(new SenderQueue("Mitto.Main"), new RequestManager());

			PublisherQueue = new ReaderQueue(ID);
			PublisherQueue.Rx += PublisherQueue_Rx;
		}

		private void PublisherQueue_Rx(object sender, RoutingFrame pFrame) {
			var objRouter = RouterCache.GetByRouterID(pFrame.DestinationID) as Router;
			if (objRouter != null) {
				objRouter.Transmit(pFrame);
			}
		}

		/// <summary>
		/// Creates a Router object that provides communication between the client
		/// and this publisher
		/// </summary>
		/// <param name="pConnection"></param>
		/// <returns></returns>
		public IRouter Create(IClientConnection pConnection) {
			return RouterCache.GetByConnection(pConnection);
		}
	}
}