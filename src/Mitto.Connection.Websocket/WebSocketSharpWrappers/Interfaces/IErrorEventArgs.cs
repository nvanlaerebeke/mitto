using System;

namespace Mitto.Connection.WebsocketSharp {
	interface IErrorEventArgs {
		Exception Exception { get; }
		string Message { get; }
	}
}
