using System;
using System.Net;

namespace Mitto.IConnection {
	public interface IServer {
		void Start(IPAddress pIPAddress, int pPort, Action<IClientConnection> pCallback);
		void Start(IPAddress pIPAddress, int pPort, string pCertPath, string pCertPassword, Action<IClientConnection> pCallback);
		void Stop();
	}
}
