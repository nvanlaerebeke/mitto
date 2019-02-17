using Mitto.IMessaging;

namespace Mitto.Messaging {
    public abstract class UnSubscribeMessage : RequestMessage {
        public UnSubscribeMessage(): base(MessageType.UnSubscribe) { }
    }
}