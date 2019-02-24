using Mitto.IMessaging;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Mitto.Messaging {
	internal interface IFrame {
		MessageType Type { get; }
		string Name { get; }
		byte[] Data { get; }
		byte[] GetByteArray();
	}
}