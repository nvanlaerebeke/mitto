﻿using Mitto.ILogging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading.Tasks;

namespace Mitto.Routing.RabbitMQ {
	public class ReaderQueue {
		private readonly ILog Log = LogFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public event EventHandler<Frame> Rx;

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

			Log.Debug($"RabbitMQ Publisher start listening on {pName}");

			var consumer = new EventingBasicConsumer(channel);
			consumer.Received += (model, ea) => {
				Task.Run(() => {
					Rx?.Invoke(this, new Frame(ea.Body));
				});
			};
			channel.BasicConsume(queue: pName, autoAck: true, consumer: consumer);
		}
	}
}