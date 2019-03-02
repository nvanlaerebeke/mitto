using Mitto.IMessaging;

namespace Mitto.Messaging.Action {
	public abstract class NotificationAction<T> : BaseAction<T>, INotificationAction where T : IMessage {
		public NotificationAction(IClient pClient, IMessage pRequest) : base(pClient, pRequest) { }

		public abstract void Start();
	}
}
