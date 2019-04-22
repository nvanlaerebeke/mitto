using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using ILogging;
using Logging;
using Mitto.IConnection;
using Mitto.Utilities;

[assembly: InternalsVisibleTo("Mitto.Connection.Websocket.Tests")]

namespace Mitto.Connection.Websocket.Server {

    internal class Client : IClientConnection {
        private readonly ILog Log = LogFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IWebSocketBehavior _objClient;
        private IKeepAliveMonitor _objKeepAliveMonitor;

        public event EventHandler<IConnection.IConnection> Disconnected;

        public event EventHandler<byte[]> Rx;

        private BlockingCollection<byte[]> _colQueue;
        private CancellationTokenSource _objCancelationSource = new CancellationTokenSource();
        private CancellationToken _objCancelationToken;

        public string ID { get; private set; }

        public Client(IWebSocketBehavior pClient, IKeepAliveMonitor pKeepAliveMonitor) {
            ID = Guid.NewGuid().ToString();

            _objClient = pClient;
            _objKeepAliveMonitor = pKeepAliveMonitor;
            _objKeepAliveMonitor.TimeOut += _objKeepAliveMonitor_TimeOut;
            _objKeepAliveMonitor.UnResponsive += _objKeepAliveMonitor_UnResponsive;

            StartTransmitQueue();

            _objClient.OnCloseReceived += _objClient_OnCloseReceived;
            _objClient.OnErrorReceived += _objClient_OnErrorReceived;
            _objClient.OnMessageReceived += _objClient_OnMessageReceived;

            _objKeepAliveMonitor.Start();

            Log.Info($"Client {ID} connected");
        }

        private void _objKeepAliveMonitor_UnResponsive(object sender, EventArgs e) {
            Log.Debug($"Client {ID} unresponsive, closing...");
            this.Disconnect();
        }

        private void _objKeepAliveMonitor_TimeOut(object sender, EventArgs e) {
            Log.Debug($"Client {ID} timeout, pinging...");
            _objKeepAliveMonitor.StartCountDown();
            if (_objClient.Ping()) {
                _objKeepAliveMonitor.Reset();
                Log.Debug($"Client {ID} pong received");
            }
        }

        private void _objClient_OnMessageReceived(object sender, IMessageEventArgs e) {
            _objKeepAliveMonitor.Reset();
            if (e.IsText) {
                Log.Debug($"Text received on {ID}");
                var data = System.Text.Encoding.UTF32.GetBytes(e.Data);
                Rx?.Invoke(this, data);
            } else if (e.IsPing) {
                Log.Debug($"Ping received on {ID}");
            } else if (e.IsBinary) {
                Log.Debug($"Data received on {ID}");
                Rx?.Invoke(this, e.RawData);
            }
        }

        private void _objClient_OnErrorReceived(object sender, IErrorEventArgs e) {
            Log.Error($"Error on {ID}: {e.Message}, closing...");
            Disconnect();
        }

        private void _objClient_OnCloseReceived(object sender, ICloseEventArgs e) {
            Disconnect();
        }

        public void Disconnect() {
            Log.Info($"Closing {ID}");

            _objKeepAliveMonitor.TimeOut -= _objKeepAliveMonitor_TimeOut;
            _objKeepAliveMonitor.UnResponsive -= _objKeepAliveMonitor_UnResponsive;

            _objClient.OnCloseReceived -= _objClient_OnCloseReceived;
            _objClient.OnErrorReceived -= _objClient_OnErrorReceived;
            _objClient.OnMessageReceived -= _objClient_OnMessageReceived;

            _objCancelationSource.Cancel();
            Disconnected?.Invoke(this, this);

            _objClient.Close();
            _objKeepAliveMonitor.Stop();
        }

        public void Transmit(byte[] pData) {
            _colQueue.Add(pData);
        }

        private void StartTransmitQueue() {
            _colQueue = new BlockingCollection<byte[]>();
            _objCancelationToken = _objCancelationSource.Token;

            //Do not use Task.Run or ThreadPool here
            //those are slower and what is needed here is a long running thread
            new Thread(() => {
                Thread.CurrentThread.Name = "SenderQueue";
                while (!_objCancelationSource.IsCancellationRequested) {
                    try {
                        var arrData = _colQueue.Take(_objCancelationToken);
                        Log.Trace($"Sending data on {ID}");
                        _objClient.Send(arrData);
                    } catch (OperationCanceledException) {
                    } catch (Exception ex) {
                        Log.Error($"Failed sending data on {ID}: {ex.Message}, closing connection");
                    }
                }
                _colQueue.Dispose(); // -- thread is exiting, clean up the collection
            }) {
                IsBackground = true
            }.Start();
        }
    }
}