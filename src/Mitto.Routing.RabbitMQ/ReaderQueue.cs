using Mitto.ILogging;
using Mitto.IRouting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading.Tasks;

namespace Mitto.Routing.RabbitMQ {
	public class ReaderQueue {
		private ILog Log {
			get {
				return LogFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
			}
		}

		public event EventHandler<RabbitMQFrame> Rx;

		public ReaderQueue(string pQueueName) {
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
			channel.QueueDeclare(queue: pName, durable: false, exclusive: false, autoDelete: false, arguments: null);

			var consumer = new EventingBasicConsumer(channel);
			consumer.Received += (model, ea) => {
				Task.Run(() => {
					Rx?.Invoke(this, new RabbitMQFrame(ea.Body));
				});
			};
			channel.BasicConsume(queue: pName, autoAck: true, consumer: consumer);
		}
	}
}