using Mitto.IMessaging;
using Mitto.IRouting;
using Mitto.Routing.RabbitMQ.Request;
using Mitto.Routing.RabbitMQ.Response;

namespace Mitto.Routing.RabbitMQ.Consumer {
	internal class MessageRouter : IRouter {
		public string ConnectionID { get; private set; }

		private readonly RoutingFrame Request;
		private readonly SenderQueue Publisher;

		public MessageRouter(string pConsumerID, SenderQueue pPublisher, RoutingFrame pFrame) {
			ConnectionID = pConsumerID;
			Publisher = pPublisher;
			Request = pFrame;
		}

		public void Start() {
			if (Request.FrameType != RoutingFrameType.Control) {
				ControlFactory.Processor.Request(new ControlRequest<CanStartActionResponse>(
					this,
					new CanStartActionRequest(Request.RequestID),
					(r) => {
						if (r.CanStart) {
							MessagingFactory.Processor.Process(this, Request.Data);
						}
					}
				));
			} else {
				if(Request.MessageType == MessageType.Response) {

				}
			}
		}

		public void Close() { }

		public void Receive(byte[] pData) {
			//nothing to do, not applicable for RabbitMQ consumer
		}

		/// <summary>
		/// Sends data back to the Publisher this request originated from
		/// ToDo: use RoutingFrame instead of byte[]
		/// </summary>
		/// <param name="pData"></param>
		public void Transmit(byte[] pData) {
			var oldFrame = new RoutingFrame(pData);
			var objFrame = new RoutingFrame(
				oldFrame.FrameType,
				oldFrame.MessageType,
				Request.RequestID,
				Consumer.ID,
				Request.DestinationID,
				oldFrame.Data
			);
			Publisher.Transmit(objFrame);
		}

		public bool IsAlive(string pRequestID) {
			return true;
		}
	}
}