using System;

namespace Mitto.Connection.Websocket {
	interface IMessageEventArgs {
		string Data { get; }
		bool IsBinary { get; }
		bool IsPing { get; }
		bool IsText { get; }
		byte[] RawData { get; }
	}
}
