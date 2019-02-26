using System;
using System.Threading;
using WebSocketSharp;

namespace Mitto.Connection.Websocket {
	internal class WebSocketClientWrapper : IWebSocketClient {
		private WebSocket _objWebSocket;

		public WebSocketClientWrapper() { }
		public WebSocketState ReadyState => _objWebSocket.ReadyState;

		public event EventHandler OnOpen;
		public event EventHandler<ICloseEventArgs> OnClose;
		public event EventHandler<IErrorEventArgs> OnError;
		public event EventHandler<IMessageEventArgs> OnMessage;

		public void Close() {
			_objWebSocket.OnOpen -= _objWebSocket_OnOpen;
			_objWebSocket.OnClose -= _objWebSocket_OnClose;
			_objWebSocket.OnError -= _objWebSocket_OnError;
			_objWebSocket.OnMessage -= _objWebSocket_OnMessage;

			if (
				_objWebSocket.ReadyState == WebSocketState.Open ||
				_objWebSocket.ReadyState == WebSocketState.Connecting
			) {
				try {
					_objWebSocket.Close();
				} catch { }
			}
		}

		/// <summary>
		/// Connects to the websocket server in an asynchonious way 
		/// </summary>
		/// <param name="pUrl"></param>
		public void ConnectAsync(string pUrl) {
			_objWebSocket = new WebSocket(pUrl) {
				WaitTime = new TimeSpan(0, 0, 30),
				EmitOnPing = true
			};
			_objWebSocket.OnOpen += _objWebSocket_OnOpen;
			_objWebSocket.OnClose += _objWebSocket_OnClose;
			_objWebSocket.OnError += _objWebSocket_OnError;
			_objWebSocket.OnMessage += _objWebSocket_OnMessage;

			_objWebSocket.ConnectAsync();
		}

		/// <summary>
		/// Raises the OnMessage event when receiving messages
		/// Note that this creates a new thread (this is to prevent deadlocks in callbacks)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void _objWebSocket_OnMessage(object sender, MessageEventArgs e) {
			OnMessage?.Invoke(sender, new MessageEventArgWrapper(e));
		}

		/// <summary>
		/// Raises the OnError event when receiving an error
		/// Note that this creates a new thread (this is to prevent deadlocks in callbacks)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void _objWebSocket_OnError(object sender, ErrorEventArgs e) {
			Close();
			OnError?.Invoke(sender, new ErrorEventArgWrapper(e));
		}

		/// <summary>
		/// Raises the OnClose event when the connection closes
		/// Note that this creates a new thread (this is to prevent deadlocks in callbacks)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void _objWebSocket_OnClose(object sender, CloseEventArgs e) {
			Close();
			OnClose?.Invoke(sender, new CloseEventArgWrapper(e));
		}

		/// <summary>
		/// Raises the OnOpen event when a connection is established
		/// Note that this creates a new thread (this is to prevent deadlocks in callbacks)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void _objWebSocket_OnOpen(object sender, EventArgs e) {
			OnOpen?.Invoke(sender, e);
		}

		/// <summary>
		/// Sends the byte[] to the websocket server
		/// </summary>
		/// <param name="pData"></param>
		public void Send(byte[] pData) {
			_objWebSocket.Send(pData);
		}

		public bool Ping() {
			return _objWebSocket.Ping();
		}
	}
}