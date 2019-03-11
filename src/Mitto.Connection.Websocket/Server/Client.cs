using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using Mitto.IConnection;
using Mitto.ILogging;
using Mitto.Utilities;

[assembly: InternalsVisibleTo("Mitto.Connection.Websocket.Tests")]
namespace Mitto.Connection.Websocket.Server {

	internal class Client : IClientConnection {
		private readonly ILog Log = LogFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private IWebSocketBehavior _objClient;
		private IKeepAliveMonitor _objKeepAliveMonitor; 

		public event EventHandler Disconnected;
        public event EventHandler<byte[]> Rx;

        private BlockingCollection<byte[]> _colQueue;
        private CancellationTokenSource _objCancelationSource = new CancellationTokenSource();
        private CancellationToken _objCancelationToken;

		public string ID => Guid.NewGuid().ToString();

		public Client(IWebSocketBehavior pClient, IKeepAliveMonitor pKeepAliveMonitor) {
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
			Log.Debug($"Data received on {ID}");
			_objKeepAliveMonitor.Reset();
			if (e.IsText) {
				var data = System.Text.Encoding.UTF32.GetBytes(e.Data);
				Rx?.Invoke(this, data);
			} else if (e.IsPing) {
			} else if (e.IsBinary) {
				Rx?.Invoke(this, e.RawData);
			}
		}

		private void _objClient_OnErrorReceived(object sender, IErrorEventArgs e) {
			Log.Debug($"Error on {ID}: {e.Message}, closing...");
			Disconnect();
		}

		private void _objClient_OnCloseReceived(object sender, ICloseEventArgs e) {
			Disconnect();
		}

        public void Disconnect() {
			Log.Debug($"Closing {ID}");

			_objKeepAliveMonitor.TimeOut -= _objKeepAliveMonitor_TimeOut;
			_objKeepAliveMonitor.UnResponsive -= _objKeepAliveMonitor_UnResponsive;

			_objClient.OnCloseReceived -= _objClient_OnCloseReceived;
			_objClient.OnErrorReceived -= _objClient_OnErrorReceived;
			_objClient.OnMessageReceived -= _objClient_OnMessageReceived;

			_objCancelationSource.Cancel();
			Disconnected?.Invoke(this, new EventArgs());

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
						Log.Error("Sending data on {ID}");
						_objClient.Send(arrData);
					} catch(OperationCanceledException) { 
					} catch (Exception ex) {
                        Log.Error("Failed sending data, closing connection: " + ex.Message);
                    }
                }
				_colQueue.Dispose(); // -- thread is exiting, clean up the collection
            }) {
                IsBackground = true
            }.Start();
        }
    }
}