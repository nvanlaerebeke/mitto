using Mitto.IMessaging;

namespace Mitto.Messaging.Base {
    public abstract class NotificationMessage : RequestMessage {
        protected NotificationMessage() : base(MessageType.Notification) { }
    }
}
