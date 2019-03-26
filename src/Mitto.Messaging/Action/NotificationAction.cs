using Mitto.IMessaging;
using Mitto.IRouting;

namespace Mitto.Messaging.Action {
	public abstract class NotificationAction<T> : BaseAction<T>, INotificationAction where T : IMessage {
		public NotificationAction(IClient pClient, T pRequest) : base(pClient, pRequest) { }

		public abstract void Start();
	}
}
