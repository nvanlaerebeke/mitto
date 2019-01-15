using Mitto.IConnection;
using System;
using System.Collections.Concurrent;
using System.Threading;
using WebSocketSharp;

namespace Connection.Websocket.Client {
	public class WebsocketClient : IClient{
		private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private string _strID;
		private WebSocket Connection;

		public string ID {
			get {
				if(String.IsNullOrEmpty(_strID)) {
					_strID = Guid.NewGuid().ToString();
				}
				return _strID;
			}
		}

		public event ConnectionHandler Connected;
		public event ConnectionHandler Disconnected;
		public event DataHandler Rx;

		#region Constructor & Connecting
		public void ConnectAsync(string pHostname, int pPort, bool pSecure) {
			Connection = new WebSocket(String.Format(((pSecure) ? "wss" : "ws") + "://{0}:{1}/", pHostname, pPort));
			Connection.OnOpen += Connection_OnOpen;
			Connection.OnClose += Connection_OnClose;
			Connection.OnError += Connection_OnError;
			Connection.OnMessage += Connection_OnMessage;

			Connection.ConnectAsync();
		}

		public void Close() {
			Connection.OnOpen -= Connection_OnOpen;
			Connection.OnClose -= Connection_OnClose;
			Connection.OnError -= Connection_OnError;
			Connection.OnMessage -= Connection_OnMessage;

			if (Connection.ReadyState != WebSocketState.Closing && Connection.ReadyState != WebSocketState.Closed) {
				Connection.Close();
			}
			Disconnected?.Invoke(this);
		}
		#endregion

		#region Websocket Event Handlers
		private void Connection_OnOpen(object sender, System.EventArgs e) {
			//Start the sending queue before we raise the event
			//The thread must be running before we say the client is 'connected(ready)'
			StartTransmitQueue(); 
			Connected?.Invoke(this);
		}

		private void Connection_OnError(object sender, WebSocketSharp.ErrorEventArgs e) {
			this.Close();
		}

		private void Connection_OnClose(object sender, CloseEventArgs e) {
			this.Close();
		}

		private void Connection_OnMessage(object sender, MessageEventArgs e) {
			Rx?.Invoke(this, e.RawData);
		}

		private BlockingCollection<byte[]> _colQueue;
		private CancellationTokenSource _objCancelationSource = new CancellationTokenSource();
		private CancellationToken _objCancelationToken;
		
		/// <summary>
		/// Transmit adds the data to be transfered to the queue
		/// </summary>
		/// <param name="pData"></param>
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
						Connection.Send(arrData);
					} catch (Exception ex) {
						Log.Error("Failed sending data, closing connection: " + ex.Message);
					}
				}
			}) {
				IsBackground = true
			}.Start();
		}
		#endregion
	}
}
