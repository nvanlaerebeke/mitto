using Mitto.IRouting;
using Mitto.Routing.Request;
using Mitto.Routing.Response;
using Mitto.Utilities;
using System;
using System.Threading.Tasks;

namespace Mitto.Routing.RabbitMQ.Publisher {

	/// <summary>
	/// ToDo: make Router a base class of this and create a:
	///    - ConnectionRouter: between IClientConnection and Publisher
	///    - ConsumerRouter: between Publisher and Consumer
	/// </summary>
	internal class ConsumerRouter : IRouter {
		public string ConnectionID { get { return Consumer.QueueName; } }

		/// <summary>
		/// Class that manages all the requests
		/// </summary>
		public readonly RequestManager RequestManager;
		public event EventHandler<IRouter> Disconnected;

		/// <summary>
		/// Client connection
		/// </summary>
		private readonly SenderQueue Consumer;
		private readonly IKeepAliveMonitor KeepAliveMonitor;

		public ConsumerRouter(SenderQueue pConsumerQueue, RequestManager pRequestManager) {
			Consumer = pConsumerQueue;
			RequestManager = pRequestManager;

			KeepAliveMonitor = new KeepAliveMonitor(60);
			KeepAliveMonitor.TimeOut += KeepAliveMonitor_TimeOut;
			KeepAliveMonitor.UnResponsive += KeepAliveMonitor_UnResponsive;
			KeepAliveMonitor.Start();
		}

		public void Receive(byte[] pData) {
			var objFrame = new RoutingFrame(pData);
			if (objFrame.FrameType == RoutingFrameType.Messaging) {
				RequestManager.Receive(this, objFrame);
			} else {
				ControlFactory.Processor.Process(this, objFrame);
			}
		}

		/// <summary>
		/// Transmits data back to the consumer
		/// ToDo: take in a RoutingFrame
		/// </summary>
		/// <param name="pData"></param>
		public void Transmit(byte[] pData) {
			var objOldFrame = new RoutingFrame(pData);
			Consumer.Transmit(new RoutingFrame(
				objOldFrame.FrameType,
				objOldFrame.MessageType,
				objOldFrame.RequestID,
				RouterProvider.ID,
				objOldFrame.SourceID,
				objOldFrame.Data
			));
		}

		public bool IsAlive(string pRequestID) {
			return (RequestManager.GetStatus(pRequestID) != MessageStatus.UnKnown);
		}

		public void Close() {
			KeepAliveMonitor.TimeOut -= KeepAliveMonitor_TimeOut;
			KeepAliveMonitor.UnResponsive -= KeepAliveMonitor_UnResponsive;
			KeepAliveMonitor.Stop();

			Consumer.Close();
			Disconnected?.Invoke(this, this);
		}

		private void KeepAliveMonitor_UnResponsive(object sender, EventArgs e) {
			Close();
		}

		private void KeepAliveMonitor_TimeOut(object sender, EventArgs e) {
			Task.Run(() => {
				Console.WriteLine("Timeout for consumer queue, sending ping...");
				ControlFactory.Processor.Request(new ControlRequest<PongResponse>(this, new PingRequest(), (PongResponse r) => {
					KeepAliveMonitor.Reset();
				}));
			});
		}
	}
}