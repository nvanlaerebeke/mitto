using System;
using System.Collections.Concurrent;
using System.Threading;
using Mitto.IConnection;

namespace Mitto.Connection.Websocket.Server {

	internal class Client : IClientConnection {
		private IWebSocketBehavior _objClient;

		public event ConnectionHandler Disconnected;
        public event DataHandler Rx;

        private BlockingCollection<byte[]> _colQueue;
        private CancellationTokenSource _objCancelationSource = new CancellationTokenSource();
        private CancellationToken _objCancelationToken;

		public string ID => _objClient.ID;

		public Client(IWebSocketBehavior pClient) {
			_objClient = pClient;
			StartTransmitQueue();
			_objClient.OnCloseReceived += _objClient_OnCloseReceived;
			_objClient.OnErrorReceived += _objClient_OnErrorReceived;
			_objClient.OnMessageReceived += _objClient_OnMessageReceived;
		}

		private void _objClient_OnMessageReceived(object sender, IMessageEventArgs e) {
			if (e.IsText) {
				var data = System.Text.Encoding.UTF8.GetBytes(e.Data);
				Rx?.Invoke(this, data);
			} else if (e.IsPing) {
			} else if (e.IsBinary) {
				Rx?.Invoke(this, e.RawData);
			}
		}

		private void _objClient_OnErrorReceived(object sender, IErrorEventArgs e) {
			Close();
		}

		private void _objClient_OnCloseReceived(object sender, ICloseEventArgs e) {
			Close();
		}

        public void Close() {
			_objClient.OnCloseReceived -= _objClient_OnCloseReceived;
			_objClient.OnErrorReceived -= _objClient_OnErrorReceived;
			_objClient.OnMessageReceived -= _objClient_OnMessageReceived;

			_objCancelationSource.Cancel();
			Disconnected?.Invoke(this);
		}

		public void Transmit(byte[] pData) {
			_colQueue.Add(pData);
		}

		private void StartTransmitQueue() {
            _colQueue = new BlockingCollection<byte[]>();
            _objCancelationToken = _objCancelationSource.Token;

            //Do not use Task.Run or ThreadPool here, those are slower and all we need is a new thread anyway
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