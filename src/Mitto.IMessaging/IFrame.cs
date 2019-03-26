using Mitto.IRouting;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Mitto.IMessaging {
	public interface IFrame {
		MessageType Type { get; }
		string Name { get; }
		byte[] Data { get; }
		byte[] GetByteArray();
	}
}