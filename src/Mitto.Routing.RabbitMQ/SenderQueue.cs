using ILogging;
using Logging;
using Mitto.IRouting;
using Mitto.Utilities;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Mitto.Routing.RabbitMQ {

    public class SenderQueue {
        public readonly string QueueName;
        public readonly QueueType QueueType;
        public readonly bool Shared;

        public event EventHandler<SenderQueue> Disconnected;

        private ILog Log => LogFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private BlockingCollection<RoutingFrame> _lstTransmitQueue = new BlockingCollection<RoutingFrame>();

        private CancellationTokenSource _objCancelationSource = new CancellationTokenSource();
        private CancellationToken _objCancelationToken;

        public SenderQueue(QueueType pType, string pQueueName, bool pShared) {
            QueueType = pType;
            QueueName = pQueueName;
            Shared = pShared;

            _objCancelationToken = _objCancelationSource.Token;
            StartSending();
        }

        /// <summary>
        /// Start the Sender
        /// This runs on a separate thread that keeps open the RabbitMQ connection
        /// so it can be reused for every byte[] received from IClientConnection
        /// </summary>
        private void StartSending() {
            new Thread(() => {
                Thread.CurrentThread.Name = $"{QueueName} Publisher";
                var objFactory = new ConnectionFactory() { HostName = "test.crazyzone.be" };
                using (var objConn = objFactory.CreateConnection()) {
                    using (var objChannel = objConn.CreateModel()) {
                        objChannel.ExchangeDeclare(exchange: QueueType.ToString(), type: "fanout");
                        objChannel.QueueDeclare(QueueName, false, false, !Shared);

                        objConn.ConnectionShutdown += Shutdown;
                        objChannel.ModelShutdown += Shutdown;

                        while (!_objCancelationSource.IsCancellationRequested) {
                            try {
                                var objFrame = _lstTransmitQueue.Take(_objCancelationToken);
                                objChannel.BasicPublish(
                                    exchange: QueueType.ToString(),
                                    routingKey: QueueName,
                                    basicProperties: null,
                                    body: objFrame.GetBytes()
                                );
                            } catch (Exception ex) {
                                Log.Error($"Failed sending data on {QueueName}: {ex.Message}, closing connection");
                            }
                        }

                        //Connection interrupted
                        objConn.ConnectionShutdown -= Shutdown;
                        objChannel.ModelShutdown -= Shutdown;
                    }
                }
            }) { IsBackground = true }.Start();
        }

        private void Shutdown(object sender, ShutdownEventArgs e) {
            Close();
        }

        public void Transmit(RoutingFrame pFrame) {
            _lstTransmitQueue.Add(pFrame);
        }

        public void Close() {
            _objCancelationSource.Cancel();
            Disconnected?.Invoke(this, this);
        }
    }
}