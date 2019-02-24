using System;

namespace Mitto.IMessaging {
	public interface IMessageConverter {
		IMessage GetMessage(Type pType, byte[] pData);
		byte[] GetByteArray(IMessage pMessage);
	}
}
