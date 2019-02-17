namespace Mitto.Connection.Websocket {
	internal class MessageEventArgWrapper : IMessageEventArgs {
		private WebSocketSharp.MessageEventArgs _objEventArgs;

		public MessageEventArgWrapper(WebSocketSharp.MessageEventArgs pEventArgs) {
			_objEventArgs = pEventArgs;
		}

		public string Data => _objEventArgs.Data;
		public bool IsBinary => _objEventArgs.IsBinary;
		public bool IsPing => _objEventArgs.IsPing;
		public bool IsText => _objEventArgs.IsText;
		public byte[] RawData => _objEventArgs.RawData;
	}
}