using System;

namespace Mitto.IConnection {
	public interface IClient: IClientConnection {
		event EventHandler<IClient> Connected;

		void ConnectAsync(string pHostname, int pPort, bool pSecure);
	}
}