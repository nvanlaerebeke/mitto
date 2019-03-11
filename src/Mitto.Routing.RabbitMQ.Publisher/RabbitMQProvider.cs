using Mitto.IConnection;
using Mitto.IRouting;

namespace Mitto.Routing.RabbitMQ.Publisher {
	public class RabbitMQProvider : IRouterProvider {
		private readonly RabbitMQParams _objParams;
		public RabbitMQProvider(RabbitMQParams pParams) {
			_objParams = pParams;
		}

		public IRouter Create(IClientConnection pConnection) {
			return new RabbitMQ(_objParams, pConnection);
		}
	}
}
