using System;

namespace Mitto.IConnection {
	public interface IClient: IClientConnection {
		event EventHandler<IClient> Connected;

		void ConnectAsync(IClientParams pParams); // string pHostname, int pPort, bool pSecure);
	}
}