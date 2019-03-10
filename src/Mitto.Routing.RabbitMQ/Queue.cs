using Mitto.ILogging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Mitto.Routing.RabbitMQ {
	public class Queue {
		private readonly ILog Log = LogFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public event EventHandler<Frame> Rx;
		private readonly string SenderQueue;
		private readonly string ReceiverQueue;

		private BlockingCollection<Frame> _lstTransmitQueue = new BlockingCollection<Frame>();

		private CancellationTokenSource _objCancelationSource = new CancellationTokenSource();
		private CancellationToken _objCancelationToken;

		public Queue(string pSenderQueue, string pReceiverQueue) {
			SenderQueue = pSenderQueue;
			ReceiverQueue = pReceiverQueue;

			StartListening();
		}

		/// <summary>
		/// Listens on the Receiver Queue for any messages
		/// 
		/// When data is received the Rx event is raised
		/// </summary>
		private void StartListening() {
			var factory = new ConnectionFactory() { HostName = "test.crazyzone.be" };
			var connection = factory.CreateConnection();
			var channel = connection.CreateModel();
			channel.QueueDeclare(queue: ReceiverQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);

			Log.Debug($"RabbitMQ Publisher start listening on {ReceiverQueue}");

			var consumer = new EventingBasicConsumer(channel);
			consumer.Received += (model, ea) => {
				Task.Run(() => {
					Rx?.Invoke(this, new Frame(ea.Body));
				});
			};
			channel.BasicConsume(queue: ReceiverQueue, autoAck: true, consumer: consumer);
		}


		/// <summary>
		/// Start the Sender 
		/// This runs on a seprate thread that keeps open the RabbitMQ connection
		/// so it can be reused for every byte[] received from IClientConnection
		/// </summary>
		private void StartSending() {
			new Thread(() => {
				var objFactory = new ConnectionFactory() { HostName = "test.crazyzone.be" };
				using (var objConn = objFactory.CreateConnection()) {
					using (var objChannel = objConn.CreateModel()) {
						while (!_objCancelationSource.IsCancellationRequested) {
							try {
								var objFrame = _lstTransmitQueue.Take(_objCancelationToken);
								objChannel.QueueDeclare(queue: SenderQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
								objChannel.BasicPublish(
									exchange: "",
									routingKey: SenderQueue,
									basicProperties: null,
									body: objFrame.Data
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
