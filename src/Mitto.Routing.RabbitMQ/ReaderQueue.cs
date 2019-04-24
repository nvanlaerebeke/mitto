using Mitto.IRouting;
using Mitto.Logging;
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
                return LoggingFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

        public readonly QueueType QueueType;
        public readonly bool Shared;
        public readonly string QueueName;

        public event EventHandler<RoutingFrame> Rx;

        public event EventHandler<ReaderQueue> Disconnected;

        public ReaderQueue(QueueType pType, string pQueueName, bool pShared) {
            QueueName = pQueueName;
            QueueType = pType;
            Shared = pShared;
            StartListening(QueueName);
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

            channel.ExchangeDeclare(exchange: QueueType.ToString(), type: "fanout");
            channel.QueueDeclare(pName, false, false, !Shared);
            channel.QueueBind(pName, QueueType.ToString(), pName);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Shutdown += Consumer_Shutdown;
            consumer.Received += (model, ea) => {
                Task.Run(() => {
                    Rx?.Invoke(this, new RoutingFrame(ea.Body));
                });
            };
            channel.BasicConsume(queue: pName, autoAck: true, consumer: consumer);
        }

        private void Consumer_Shutdown(object sender, ShutdownEventArgs e) {
            Disconnected?.Invoke(this, this);
        }
    }
}