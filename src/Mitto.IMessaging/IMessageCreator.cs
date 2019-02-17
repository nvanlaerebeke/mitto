namespace Mitto.IMessaging {
	public interface IMessageCreator {
		IMessage Create(byte[] pData);
		byte[] GetBytes(IMessage pMessage);
		IMessage GetResponseMessage(IMessage pMessage, ResponseCode pCode);
	}
}
