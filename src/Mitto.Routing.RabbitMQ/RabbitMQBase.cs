using Mitto.ILogging;
using Mitto.IRouting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Mitto.Routing.RabbitMQ {
	public abstract class RabbitMQBase : IRouter {
		protected readonly ILog Log = LogFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public abstract void Transmit(byte[] pMessage);
		public abstract void Receive(byte[] pMessage);

		private BlockingCollection<Message> _colRxQueue = new BlockingCollection<Message>();
		private BlockingCollection<Message> _colTxQueue = new BlockingCollection<Message>();

		private CancellationTokenSource _objCancelationSource = new CancellationTokenSource();
		private CancellationToken _objCancelationToken;

		protected string ReadQueue { get; set; }

		public string ID { get; } = Guid.NewGuid().ToString();

		public RabbitMQBase(string pReadQueue) {
			ReadQueue = pReadQueue;

			_objCancelationToken = _objCancelationSource.Token;

			StartTxQueue();
			StartRxQueue();
		}

		#region Configuration
		protected static Config Config;

		public event EventHandler<byte[]> Rx;

		public static void SetConfig(Config pConfig) {
			Config = pConfig;
		}
		#endregion


		#region Rx
		
		private void StartRxQueue() {
			new Thread(() => {
				var factory = new ConnectionFactory() { HostName = Config.Host };
				var connection = factory.CreateConnection();
				var channel = connection.CreateModel();
				channel.QueueDeclare(queue: ReadQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);

				//Log.Debug("Start listening on " + ReadQueue);

				var consumer = new EventingBasicConsumer(channel);
				consumer.Received += (model, ea) => {
					var obj = new Message(ReadQueue, ea.Body);
					Receive(obj.Data);
				};
				channel.BasicConsume(queue: ReadQueue, autoAck: true, consumer: consumer);
			}).Start();
		}
		#endregion

		#region Tx


		protected void AddToTxQueue(Message pMessage) {
			_colTxQueue.Add(pMessage);
		}

		private void StartTxQueue() {
			new Thread(() => {
				Thread.CurrentThread.Name = "Tx_RabbitMQ";
				var objFactory = new ConnectionFactory() { HostName = Config.Host };
				using (var objConn = objFactory.CreateConnection()) {
					using (var objChannel = objConn.CreateModel()) {
						while (!_objCancelationSource.IsCancellationRequested) {
							try {
								var objMessage = _colTxQueue.Take(_objCancelationToken);
								objChannel.QueueDeclare(queue: objMessage.ClientID, durable: false, exclusive: false, autoDelete: false, arguments: null);
								objChannel.BasicPublish(
									exchange: "",
									routingKey: objMessage.ClientID,
									basicProperties: null,
									body: objMessage.Data
								);
							} catch (Exception ex) {
								Log.Error($"Failed sending data, closing connection: {ex.Message}");
							}
						}
					}
				}
			}) {
				IsBackground = true
			}.Start();
		}

		public void Close() {
			throw new NotImplementedException();
		}
		#endregion
	}
}