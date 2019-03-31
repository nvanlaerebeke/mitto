using Mitto.ILogging;
using Mitto.IRouting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading.Tasks;

namespace Mitto.Routing.RabbitMQ {
	/// <summary>
	/// ToDo: Add an option to make a read/write queue exclusive
	/// The read queues for the publishers will be exclusive
	/// Same for the Consumers
	/// </summary>
	public class ReaderQueue {
		private ILog Log {
			get {
				return LogFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
			}
		}

		public readonly QueueType QueueType;
		public readonly bool Shared;
		public event EventHandler<RoutingFrame> Rx;
		public event EventHandler<ReaderQueue> Disconnected;

		public ReaderQueue(QueueType pType, string pQueueName, bool pShared) {
			QueueType = pType;
			Shared = pShared;
			StartListening(pQueueName);
		}

		/// <summary>
		/// Listens on the Receiver Queue for any messages
		/// 
		/// When data is received the Rx event is raised
		/// </summary>
		private void StartListening(string pName) {
			var factory = new ConnectionFactory() { HostName = "test.crazyzone.be" };
			var connection = factory.CreateConnection();
			var channel = connection.CreateModel();

			connection.ConnectionShutdown += Connection_ConnectionShutdown;

			channel.ExchangeDeclare(exchange: QueueType.ToString(), type: "fanout");
			channel.QueueDeclare(pName, false, false, !Shared);
			channel.QueueBind(pName, QueueType.ToString(), pName);
			channel.ModelShutdown += Channel_ModelShutdown;

			var consumer = new EventingBasicConsumer(channel);
			consumer.ConsumerCancelled += Consumer_ConsumerCancelled;
			consumer.Shutdown += Consumer_Shutdown;
			consumer.Received += (model, ea) => {
				Task.Run(() => {
					Rx?.Invoke(this, new RoutingFrame(ea.Body));
				});
			};
			channel.BasicConsume(queue: pName, autoAck: true, consumer: consumer);
		}

		private void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e) {
			throw new NotImplementedException();
		}

		private void Consumer_ConsumerCancelled(object sender, ConsumerEventArgs e) {
			throw new NotImplementedException();
		}

		private void Channel_ModelShutdown(object sender, ShutdownEventArgs e) {
			throw new NotImplementedException();
		}

		private void Consumer_Shutdown(object sender, ShutdownEventArgs e) {
			Disconnected?.Invoke(this, this);
		}
	}
}