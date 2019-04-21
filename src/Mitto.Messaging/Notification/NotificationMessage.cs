using Mitto.IMessaging;
using Mitto.IRouting;

namespace Mitto.Messaging {
    public abstract class NotificationMessage : RequestMessage, INotificationMessage {
        protected NotificationMessage() : base(MessageType.Notification) { }
    }
}
