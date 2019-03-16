/// <summary>
/// ToDo: Add downstream KeepAlive that check if the request is still being processed by the IConnection
/// Will need to pass the actual connection instead of the connection id
/// </summary>
namespace Mitto.Routing.RabbitMQ.Publisher {
	internal class Request {
		public readonly string ConnectionID;
		public readonly string RequestID;
		private readonly SenderQueue ConsumerQueue;

		public Request(string pConnectionID, string pRequestID, SenderQueue pConsumerQueue) {
			ConnectionID = pConnectionID;
			RequestID = pRequestID;
			ConsumerQueue = pConsumerQueue;
		}

		public void SetResponse(Frame pFrame) {
			ConsumerQueue.Transmit(pFrame);
		}
	}
}