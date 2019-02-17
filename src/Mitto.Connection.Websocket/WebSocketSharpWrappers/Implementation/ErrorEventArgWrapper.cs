using System;

namespace Mitto.Connection.Websocket {
	internal class ErrorEventArgWrapper : IErrorEventArgs {
		private WebSocketSharp.ErrorEventArgs _objEventArgs;

		public ErrorEventArgWrapper(WebSocketSharp.ErrorEventArgs pEventArgs) {
			_objEventArgs = pEventArgs;
		}

		public string Message => _objEventArgs.Message;
		public Exception Exception => _objEventArgs.Exception;
	}
}