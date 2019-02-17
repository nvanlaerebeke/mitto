namespace Mitto.Connection.Websocket {
	internal class CloseEventArgWrapper : ICloseEventArgs {
		private WebSocketSharp.CloseEventArgs _objEventArgs;

		public CloseEventArgWrapper(WebSocketSharp.CloseEventArgs pEventArgs) {
			_objEventArgs = pEventArgs;
		}

		public ushort Code => _objEventArgs.Code;
		public string Reason => _objEventArgs.Reason;
		public bool WasClean => _objEventArgs.WasClean;
	}
}