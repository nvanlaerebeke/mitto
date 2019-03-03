using System;

namespace Mitto.IConnection {
	public interface IClientConnection: IConnection {
		void Transmit(byte[] pData);
	}
}
