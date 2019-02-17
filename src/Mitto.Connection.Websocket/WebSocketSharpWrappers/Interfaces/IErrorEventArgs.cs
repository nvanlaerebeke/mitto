using System;

namespace Mitto.Connection.Websocket {
	interface IErrorEventArgs {
		Exception Exception { get; }
		string Message { get; }
	}
}
