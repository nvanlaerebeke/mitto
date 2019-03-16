using System;

namespace Mitto.IConnection {
	public interface IConnection {
		event EventHandler<IConnection> Disconnected;
		event EventHandler<byte[]> Rx;
		string ID { get; }

		void Disconnect();
	}
}
