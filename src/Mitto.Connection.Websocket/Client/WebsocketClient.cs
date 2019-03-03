using Mitto.IConnection;
using Mitto.Utilities;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using WebSocketSharp;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
/// <summary>
/// Class that represents the Websocket Client in Mitto
/// Provides functionallity to communicate with a websocket server
/// </summary>
namespace Mitto.Connection.Websocket.Client {
	public class WebsocketClient : IClient {
		private IWebSocketClient _objWebSocketClient;
		private IKeepAliveMonitor _objKeepAliveMonitor;

		public string ID { get; private set; } = Guid.NewGuid().ToString();

		public event EventHandler<IClient> Connected;
		public event EventHandler<IConnection.IConnection> Disconnected;
		public event DataHandler Rx;

		internal WebsocketClient(IWebSocketClient pWebSocket, IKeepAliveMonitor pKeepAliveMonitor) {
			_objWebSocketClient = pWebSocket;
			_objKeepAliveMonitor = pKeepAliveMonitor;
			_objKeepAliveMonitor.TimeOut += _objKeepAliveMonitor_TimeOut;
			_objKeepAliveMonitor.UnResponsive += _objKeepAliveMonitor_UnResponsive;
		}

		#region Constructor & Connecting
		public void ConnectAsync(IClientParams pParams) { //string pHostname, int pPort, bool pSecure) {
			if (!(pParams is ClientParams objParams)) {
				throw new Exception("Incorrect parameters for Websocket client");
			}

			_objWebSocketClient.OnOpen += Connection_OnOpen;
			_objWebSocketClient.OnClose += Connection_OnClose;
			_objWebSocketClient.OnError += Connection_OnError;
			_objWebSocketClient.OnMessage += Connection_OnMessage;

			_objWebSocketClient.ConnectAsync(String.Format(((objParams.Secure) ? "wss" : "ws") + "://{0}:{1}/", objParams.Hostname, objParams.Port));
		}

		private void Close() {
			_objKeepAliveMonitor.TimeOut -= _objKeepAliveMonitor_TimeOut;
			_objKeepAliveMonitor.UnResponsive -= _objKeepAliveMonitor_UnResponsive;

			_objWebSocketClient.OnOpen -= Connection_OnOpen;
			_objWebSocketClient.OnClose -= Connection_OnClose;
			_objWebSocketClient.OnError -= Connection_OnError;
			_objWebSocketClient.OnMessage -= Connection_OnMessage;

			_objCancelationSource.Cancel();
			if (_objWebSocketClient.ReadyState != WebSocketState.Closing && _objWebSocketClient.ReadyState != WebSocketState.Closed) {
				_objWebSocketClient.Close();
			}
			_objKeepAliveMonitor.Stop();
		}

		private void _objKeepAliveMonitor_TimeOut(object sender, EventArgs e) {
			_objKeepAliveMonitor.StartCountDown();
			if (_objWebSocketClient.Ping()) {
				_objKeepAliveMonitor.Reset();
			}
		}

		private void _objKeepAliveMonitor_UnResponsive(object sender, EventArgs e) {
			this.Disconnect();
		}
		#endregion

		#region Websocket Event Handlers
		private void Connection_OnOpen(object sender, EventArgs e) {
			//Start the sending queue before we raise the event
			//The thread must be running before we say the client is 'connected(ready)'
			StartTransmitQueue();
			_objKeepAliveMonitor.Start();
			Connected?.Invoke(this, this);
		}

		private void Connection_OnError(object sender, IErrorEventArgs e) {
			this.Close();
			Disconnected?.Invoke(this, this);
		}

		private void Connection_OnClose(object sender, ICloseEventArgs e) {
			this.Close();
			Disconnected?.Invoke(this, this);
		}

		private void Connection_OnMessage(object sender, IMessageEventArgs e) {
			_objKeepAliveMonitor.Reset();
			if (e.IsText) {
				var data = System.Text.Encoding.UTF32.GetBytes(e.Data);
				Rx?.Invoke(this, data);
			} else if (e.IsPing) { // -- do nothing, keepalive is handled in this class
			} else if (e.IsBinary) {
				Rx?.Invoke(this, e.RawData);
			}
		}
		#endregion

		#region Client Methods
		public void Disconnect() {
			this.Close();
			Disconnected?.Invoke(this, this);
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
					} catch (Exception) {
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
