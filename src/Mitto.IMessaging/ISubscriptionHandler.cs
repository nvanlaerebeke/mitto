namespace Mitto.IMessaging {
	/// <summary>
	/// ToDo: Replace T IRequestMessage with S ISubscribeMessage and U IUnSubscribeMessage
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="N"></typeparam>
	public interface ISubscriptionHandler<S, U, N> 
		where S : IRequestMessage
		where U : IRequestMessage
		where N : IRequestMessage
	{
		bool Sub(IClient pClient, S pMessage);
		bool UnSub(IClient pClient, U pMessage);

		bool Notify(IClient pSender, N pNotifyMessage);
	}
}