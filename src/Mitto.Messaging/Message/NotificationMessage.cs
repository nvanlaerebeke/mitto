using Mitto.IMessaging;

namespace Mitto.Messaging {
    public abstract class NotificationMessage : RequestMessage {
        protected NotificationMessage() : base(MessageType.Notification) { }
    }
}
