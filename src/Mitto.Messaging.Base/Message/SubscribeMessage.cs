using Mitto.IMessaging;

namespace Mitto.Messaging.Base {
    public abstract class SubscribeMessage : RequestMessage {
        public SubscribeMessage(): base(MessageType.Subscribe) { }
    }
}
