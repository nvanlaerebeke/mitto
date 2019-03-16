using Mitto.IConnection;
using Mitto.IRouting;

namespace Mitto.Routing.RabbitMQ.Publisher {
	public class RabbitMQProvider : IRouterProvider {
		private readonly string PublisherID;
		private readonly SenderQueue MainQueue;
		private readonly RabbitMQParams _objParams;

		public RabbitMQProvider(RabbitMQParams pParams) {
			_objParams = pParams;
			PublisherID = "Mitto.Publisher." + System.Guid.NewGuid().ToString();
			MainQueue = new SenderQueue("Mitto.Main");
		}

		public IRouter Create(IClientConnection pConnection) {
			return new Router(MainQueue, PublisherID, pConnection);
		}
	}
}