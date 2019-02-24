using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Mitto.Messaging.Json {
	internal interface IFrame {
		MessageFormat Format { get; }
		byte[] Data { get; }
		byte[] GetByteArray();
	}
}