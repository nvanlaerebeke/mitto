namespace IMessaging {
	public interface IMessageCreator {
		IMessage Create(byte[] pData);
		byte[] GetBytes(IMessage pMessage);
	}
}
