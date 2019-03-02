namespace Mitto.IMessaging {
	/// <summary>
	/// Providers the messages and actions for the specific provider
	/// 
	/// Implement this interface to use a custom MessageProvider
	/// this is done when implementing your own custom way of how
	/// messages are represented(IMessage/IResponseMessage) and how they are handled (IAction)
	/// </summary>
	public interface IMessageProvider {
		IMessage GetMessage(byte[] pData);
		IResponseMessage GetResponseMessage(IMessage pMessage, ResponseCode pCode);
		IAction GetAction(IClient pClient, IMessage pMessage);
		byte[] GetByteArray(IMessage pMessage);
	}
}