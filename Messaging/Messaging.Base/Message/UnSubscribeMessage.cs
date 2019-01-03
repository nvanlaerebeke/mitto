using IMessaging;

namespace Messaging.Base {
    public abstract class UnSubscribeMessage : RequestMessage {
        public UnSubscribeMessage(): base(MessageType.UnSubscribe) { }
    }
}