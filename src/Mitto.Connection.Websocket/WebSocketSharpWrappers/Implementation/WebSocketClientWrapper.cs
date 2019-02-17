using System;
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

		public void ConnectAsync(string pUrl) {
			_objWebSocket = new WebSocket(pUrl);
			_objWebSocket.OnOpen += _objWebSocket_OnOpen;
			_objWebSocket.OnClose += _objWebSocket_OnClose;
			_objWebSocket.OnError += _objWebSocket_OnError;
			_objWebSocket.OnMessage += _objWebSocket_OnMessage;
		}

		private void _objWebSocket_OnMessage(object sender, MessageEventArgs e) {
			OnMessage?.Invoke(sender, new MessageEventArgWrapper(e));
		}

		private void _objWebSocket_OnError(object sender, ErrorEventArgs e) {
			Close();
			OnError?.Invoke(sender, new ErrorEventArgWrapper(e));
		}

		private void _objWebSocket_OnClose(object sender, CloseEventArgs e) {
			Close();
			OnClose?.Invoke(sender, new CloseEventArgWrapper(e));
		}

		private void _objWebSocket_OnOpen(object sender, EventArgs e) {
			OnOpen?.Invoke(sender, e);
		}

		public void Send(byte[] pData) {
			_objWebSocket.Send(pData);
		}
	}
}
