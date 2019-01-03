using IMessaging;

namespace Messaging.Base {
    public abstract class SubscribeMessage : RequestMessage {
        public SubscribeMessage(): base(MessageType.Subscribe) { }
    }
}
