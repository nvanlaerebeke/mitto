using Mitto.IMessaging;
using Mitto.IRouting;

namespace Mitto.Messaging {
    public abstract class NotificationMessage : RequestMessage {
        protected NotificationMessage() : base(MessageType.Notification) { }
    }
}
