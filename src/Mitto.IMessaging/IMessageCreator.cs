namespace Mitto.IMessaging {
	public interface IMessageConverter {
		IMessage GetMessage(byte[] pData);
		IResponseMessage GetResponseMessage(IMessage pMessage, ResponseCode pCode);
		byte[] GetByteArray(IMessage pMessage);
	}
}
