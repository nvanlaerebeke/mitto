using IMessaging;

namespace Messaging.Base {
    public abstract class NotificationMessage : RequestMessage {
        protected NotificationMessage() : base(MessageType.Notification) { }
    }
}
