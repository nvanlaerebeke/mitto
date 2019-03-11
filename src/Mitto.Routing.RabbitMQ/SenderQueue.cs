using Mitto.ILogging;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Mitto.Routing.RabbitMQ {
	public class SenderQueue {
		private readonly ILog Log = LogFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private BlockingCollection<Frame> _lstTransmitQueue = new BlockingCollection<Frame>();

		private CancellationTokenSource _objCancelationSource = new CancellationTokenSource();
		private CancellationToken _objCancelationToken;

		public SenderQueue(string pQueueName) {
			StartSending(pQueueName);
		}

		/// <summary>
		/// Start the Sender 
		/// This runs on a seprate thread that keeps open the RabbitMQ connection
		/// so it can be reused for every byte[] received from IClientConnection
		/// </summary>
		private void StartSending(string pName) {
			new Thread(() => {
				var objFactory = new ConnectionFactory() { HostName = "test.crazyzone.be" };
				using (var objConn = objFactory.CreateConnection()) {
					using (var objChannel = objConn.CreateModel()) {
						while (!_objCancelationSource.IsCancellationRequested) {
							try {
								var objFrame = _lstTransmitQueue.Take(_objCancelationToken);
								objChannel.QueueDeclare(queue: pName, durable: false, exclusive: false, autoDelete: false, arguments: null);
								objChannel.BasicPublish(
									exchange: "",
									routingKey: pName,
									basicProperties: null,
									body: objFrame.GetBytes()
								);
							} catch (Exception ex) {
								Log.Error($"Failed sending data, closing connection: {ex.Message}");
							}
						}
					}
				}
			}).Start();
		}

		public void Transmit(Frame pFrame) {
			_lstTransmitQueue.Add(pFrame);
		}

		public void Close() {
			_objCancelationSource.Cancel();
		}
	}
}
