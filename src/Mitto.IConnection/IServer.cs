using System;
using System.Net;

namespace Mitto.IConnection {
	public interface IServer {
		void Start(IServerParams pParams, Action<IClientConnection> pClientConnectedAction);
		void Stop();
	}
}