using System;
using System.Collections.Concurrent;
using System.Threading;
using WebSocketSharp;
using WebSocketSharp.Server;
using Mitto.IConnection;

namespace Mitto.Connection.Websocket.Server {

    internal class Client : WebSocketBehavior, IClientConnection {
		private new static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public event ConnectionHandler Disconnected;
        public event DataHandler Rx;

        private BlockingCollection<byte[]> _colQueue;
        private CancellationTokenSource _objCancelationSource = new CancellationTokenSource();
        private CancellationToken _objCancelationToken;

		public static Action<IClientConnection> ClientConnectCallback { get; internal set; }

		protected override void OnOpen() {
            base.OnOpen();
            StartTransmitQueue();
			ClientConnectCallback.Invoke(this);
        }

        protected override void OnError(ErrorEventArgs e) {
            base.OnError(e);
            _objCancelationSource.Cancel();
			Close();
        }

        protected override void OnMessage(MessageEventArgs e) {
            if (e.IsText) {
                var data = System.Text.Encoding.UTF8.GetBytes(e.Data);
                Rx?.Invoke(this, data);
            } else if (e.IsPing) {
            } else if (e.IsBinary) {
                Rx?.Invoke(this, e.RawData);
            }
        }

        protected override void OnClose(CloseEventArgs e) {
            base.OnClose(e);
            _objCancelationSource.Cancel();
            Disconnected?.Invoke(this);
        }

        public void Transmit(byte[] pData) {
            _colQueue.Add(pData);
        }

        public new void Close() {
            _objCancelationSource.Cancel();
            _colQueue.Dispose();
			base.Close();
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
						base.Send(arrData);
                    } catch (Exception ex) {
                        Log.Error("Failed sending data, closing connection: " + ex.Message);
                    }
                }
            }) {
                IsBackground = true
            }.Start();
        }
    }
}