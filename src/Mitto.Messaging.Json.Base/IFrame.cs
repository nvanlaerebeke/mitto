using Mitto.IMessaging;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Mitto.Messaging.Json {
	internal interface IFrame {
		MessageFormat Format { get; }
		MessageType Type { get; }
		byte Code { get; }
		byte[] Data { get; }
		byte[] GetByteArray();
	}
}