using Mitto.IMessaging;

namespace Mitto.Messaging.Json {
	internal interface IFrame {
		MessageFormat Format { get; }
		MessageType Type { get; }
		byte Code { get; }
		byte[] Data { get; }
		byte[] GetByteArray();
	}
}