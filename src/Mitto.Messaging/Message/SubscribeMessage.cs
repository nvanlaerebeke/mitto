using Mitto.IMessaging;

namespace Mitto.Messaging {
    public abstract class SubscribeMessage : RequestMessage {
        public SubscribeMessage(): base(MessageType.Subscribe) { }
    }
}
