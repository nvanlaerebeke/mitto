using Mitto.IMessaging;

namespace Mitto.Messaging.Base {
    public abstract class UnSubscribeMessage : RequestMessage {
        public UnSubscribeMessage(): base(MessageType.UnSubscribe) { }
    }
}