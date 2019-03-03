using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using Mitto.IConnection;
using Mitto.Utilities;

[assembly: InternalsVisibleTo("Mitto.Connection.Websocket.Tests")]
namespace Mitto.Connection.Websocket.Server {

	internal class Client : IClientConnection {
		private IWebSocketBehavior _objClient;
		private IKeepAliveMonitor _objKeepAliveMonitor; 

		public event EventHandler<IConnection.IConnection> Disconnected;
        public event DataHandler Rx;

        private BlockingCollection<byte[]> _colQueue;
        private CancellationTokenSource _objCancelationSource = new CancellationTokenSource();
        private CancellationToken _objCancelationToken;

		public string ID => _objClient.ID;

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
		}

		private void _objKeepAliveMonitor_UnResponsive(object sender, EventArgs e) {
			this.Disconnect();
		}

		private void _objKeepAliveMonitor_TimeOut(object sender, EventArgs e) {
			_objKeepAliveMonitor.StartCountDown();
			if (_objClient.Ping()) {
				_objKeepAliveMonitor.Reset();
			}
		}

		private void _objClient_OnMessageReceived(object sender, IMessageEventArgs e) {
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
			Disconnect();
		}

		private void _objClient_OnCloseReceived(object sender, ICloseEventArgs e) {
			Disconnect();
		}

        public void Disconnect() {
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

		/// <summary>
		/// ToDo: test vs SendAsync
		/// </summary>
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
						_objClient.Send(arrData);
                    } catch (Exception ex) {
                        //Log.Error("Failed sending data, closing connection: " + ex.Message);
                    }
                }
				_colQueue.Dispose(); // -- thread is exiting, clean up the collection
            }) {
                IsBackground = true
            }.Start();
        }
    }
}