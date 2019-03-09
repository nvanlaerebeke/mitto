using Mitto.IConnection;
using Mitto.ILogging;
using Mitto.Utilities;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using WebSocketSharp;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
/// <summary>
/// Class that represents the Websocket Client in Mitto
/// Provides functionality to communicate with a websocket server
/// </summary>
namespace Mitto.Connection.Websocket.Client {
	public class WebsocketClient : IClient {
		private readonly ILog Log = LogFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private IWebSocketClient _objWebSocketClient;
		private IKeepAliveMonitor _objKeepAliveMonitor;

		public static int FragmentLength {
			get { return WebSocket.FragmentLength; }
			set { WebSocket.FragmentLength = value; }
		}

		public string ID { get; private set; } = Guid.NewGuid().ToString();
		public long CurrentBytesPerSecond { get { return _objWebSocketClient.CurrentBytesPerSecond; } }

		public event EventHandler<IClient> Connected;
		public event EventHandler Disconnected;
		public event EventHandler<byte[]> Rx;

		internal WebsocketClient(IWebSocketClient pWebSocket, IKeepAliveMonitor pKeepAliveMonitor) {
			_objWebSocketClient = pWebSocket;
			_objKeepAliveMonitor = pKeepAliveMonitor;
			_objKeepAliveMonitor.TimeOut += _objKeepAliveMonitor_TimeOut;
			_objKeepAliveMonitor.UnResponsive += _objKeepAliveMonitor_UnResponsive;
		}

		#region Constructor & Connecting
		public void ConnectAsync(IClientParams pParams) {
			if (!(pParams is ClientParams objParams)) {
				Log.Error("Incorrect parameters for Websocket client");
				throw new Exception("Incorrect parameters for Websocket client");
			}
			Log.Info($"Connecting {ID} to {objParams.Hostname}:{objParams.Port}");

			_objWebSocketClient.ConnectionTimeoutSeconds = objParams.ConnectionTimeoutSeconds;
			_objKeepAliveMonitor.SetInterval(objParams.ConnectionTimeoutSeconds);

			_objWebSocketClient.OnOpen += Connection_OnOpen;
			_objWebSocketClient.OnClose += Connection_OnClose;
			_objWebSocketClient.OnError += Connection_OnError;
			_objWebSocketClient.OnMessage += Connection_OnMessage;

			_objWebSocketClient.ConnectAsync(objParams);
		}

		private void Close() {
			Log.Info($"Closing connection: {ID}");
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
			Log.Debug($"Connection timeout: {ID}, pinging...");
			_objKeepAliveMonitor.StartCountDown();
			if (_objWebSocketClient.Ping()) {
				_objKeepAliveMonitor.Reset();
				Log.Debug($"pong received: {ID}");
			}
		}

		private void _objKeepAliveMonitor_UnResponsive(object sender, EventArgs e) {
			Log.Info($"Connection {ID} lost, closing...");
			this.Disconnect();
		}
		#endregion

		#region Websocket Event Handlers
		private void Connection_OnOpen(object sender, EventArgs e) {
			Log.Info($"Connection {ID} connected");

			//Start the sending queue before we raise the event
			//The thread must be running before we say the client is 'connected(ready)'
			StartTransmitQueue();
			_objKeepAliveMonitor.Start();
			Connected?.Invoke(this, this);
		}

		private void Connection_OnError(object sender, IErrorEventArgs e) {
			Log.Error($"Connection error for {ID}: {e.Message}");
			this.Close();
			Disconnected?.Invoke(this, new EventArgs());
		}

		private void Connection_OnClose(object sender, ICloseEventArgs e) {
			Log.Info($"Connection {ID} closed");
			this.Close();
			Disconnected?.Invoke(this, new EventArgs());
		}

		private void Connection_OnMessage(object sender, IMessageEventArgs e) {
			Log.Debug($"Data received on {ID}");
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
			Disconnected?.Invoke(this, new EventArgs());
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
						Log.Debug($"Sending Data on {ID}");
						_objWebSocketClient.Send(arrData);
					} catch (Exception ex) {
						Log.Error($"Failed sending data, closing connection: {ex.Message}");
					}
				}
			}) {
				IsBackground = true
			}.Start();
		}
		#endregion
	}
}
