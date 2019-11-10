using System;

namespace Mitto.Connection.WebsocketSharp {
	interface IMessageEventArgs {
		string Data { get; }
		bool IsBinary { get; }
		bool IsPing { get; }
		bool IsText { get; }
		byte[] RawData { get; }
	}
}
