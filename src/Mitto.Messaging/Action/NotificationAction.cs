using Mitto.IMessaging;

namespace Mitto.Messaging.Action {

    public abstract class NotificationAction<I> : BaseAction<I>, INotificationAction<I> where I : IMessage {

        public NotificationAction(IClient pClient, I pRequest) : base(pClient, pRequest) {
        }

        public abstract void Start();
    }
}