using Mitto.IQueue;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Queue.RabbitMQ {
	public abstract class RabbitMQBase : IQueue {
		protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public abstract void Receive(Message pMessage);

		private BlockingCollection<Message> _colRxQueue = new BlockingCollection<Message>();
		private BlockingCollection<Message> _colTxQueue = new BlockingCollection<Message>();

		private CancellationTokenSource _objCancelationSource = new CancellationTokenSource();
		private CancellationToken _objCancelationToken;

		protected string ReadQueue { get; set; }

		public RabbitMQBase(string pReadQueue) {
			ReadQueue = pReadQueue;

			_objCancelationToken = _objCancelationSource.Token;

			StartTxQueue();
			StartRxQueue();
		}


		#region Configuration
		protected static Config Config;
		public static void SetConfig(Config pConfig) {
			Config = pConfig;
		}
		#endregion

		#region Rx
		public abstract event DataHandler Rx;
		private void StartRxQueue() {
			new Thread(() => {
				var factory = new ConnectionFactory() { HostName = Config.Host };
				var connection = factory.CreateConnection();
				var channel = connection.CreateModel();
				channel.QueueDeclare(queue: ReadQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);

				Log.Debug("Start listening on " + ReadQueue);

				var consumer = new EventingBasicConsumer(channel);
				consumer.Received += (model, ea) => {
					var obj = new Message(ReadQueue, ea.Body);
					Receive(obj);
				};
				channel.BasicConsume(queue: ReadQueue, autoAck: true, consumer: consumer);
			}).Start();
		}
		#endregion

		#region Tx
		/// <summary>
		/// Transmit data from Client -> Message Handler (Tx)
		/// </summary>
		/// <param name="pMessage"></param>
		public abstract void Transmit(Message pMessage);

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
								Log.Error("Failed sending data, closing connection: " + ex.Message);
							}
						}
					}
				}
			}) {
				IsBackground = true
			}.Start();
		}
		#endregion
	}
}
