using Mitto.IMessaging;
using Mitto.IRouting;

/// <summary>
/// ToDo: Add downstream KeepAlive that check if the request is still being processed by the IConnection
/// Will need to pass the actual connection instead of the connection id
///
/// Combine with Request
/// </summary>
namespace Mitto.Routing.RabbitMQ {

	public class ControlRequest : IRequest {
		public readonly string ConnectionID;
		public readonly string RequestID;
		private readonly SenderQueue ConsumerQueue;

		public MessageStatus Status { get; set; } = MessageStatus.UnKnown;

		public string ID => System.Guid.NewGuid().ToString();

		public ControlRequest(string pConnectionID, string pRequestID, SenderQueue pConsumerQueue) {
			ConnectionID = pConnectionID;
			RequestID = pRequestID;
			ConsumerQueue = pConsumerQueue;
		}

		public void SetResponse(RoutingFrame pFrame) {
			//ConsumerQueue.Transmit(objRabbitMQFrame);
		}

		public void Send() {
			
		}
	}
}