using Mitto.IConnection;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using WebSocketSharp;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Mitto.Connection.Websocket.Client {
	public class WebsocketClient : IClient {
		private IWebSocketClient _objWebSocketClient;

		public string ID { get; private set; } = Guid.NewGuid().ToString();

		public event ConnectionHandler Connected;
		public event ConnectionHandler Disconnected;
		public event DataHandler Rx;

		internal WebsocketClient(IWebSocketClient pWebSocket) {
			_objWebSocketClient = pWebSocket;
		}

		#region Constructor & Connecting
		public void ConnectAsync(string pHostname, int pPort, bool pSecure) {
			_objWebSocketClient.OnOpen += Connection_OnOpen;
			_objWebSocketClient.OnClose += Connection_OnClose;
			_objWebSocketClient.OnError += Connection_OnError;
			_objWebSocketClient.OnMessage += Connection_OnMessage;

			_objWebSocketClient.ConnectAsync(String.Format(((pSecure) ? "wss" : "ws") + "://{0}:{1}/", pHostname, pPort));
		}

		public void Close() {
			_objWebSocketClient.OnOpen -= Connection_OnOpen;
			_objWebSocketClient.OnClose -= Connection_OnClose;
			_objWebSocketClient.OnError -= Connection_OnError;
			_objWebSocketClient.OnMessage -= Connection_OnMessage;

			if (_objWebSocketClient.ReadyState != WebSocketState.Closing && _objWebSocketClient.ReadyState != WebSocketState.Closed) {
				_objWebSocketClient.Close();
			}
		}
		#endregion

		#region Websocket Event Handlers
		private void Connection_OnOpen(object sender, EventArgs e) {
			//Start the sending queue before we raise the event
			//The thread must be running before we say the client is 'connected(ready)'
			StartTransmitQueue(); 
			Connected?.Invoke(this);
		}

		private void Connection_OnError(object sender, IErrorEventArgs e) {
			this.Close();
			Disconnected?.Invoke(this);
		}

		private void Connection_OnClose(object sender, ICloseEventArgs e) {
			this.Close();
			Disconnected?.Invoke(this);	
		}

		private void Connection_OnMessage(object sender, IMessageEventArgs e) {
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

			//Do not use Task.Run or ThreadPool here, we need a long running thread for the SenderQueue
			new Thread(() => {
				Thread.CurrentThread.Name = "SenderQueue";
				while (!_objCancelationSource.IsCancellationRequested) {
					try {
						var arrData = _colQueue.Take(_objCancelationToken);
						_objWebSocketClient.Send(arrData);
					} catch (Exception ex) {
						//Log.Error("Failed sending data, closing connection: " + ex.Message);
					}
				}
			}) {
				IsBackground = true
			}.Start();
		}
		#endregion
	}
}
