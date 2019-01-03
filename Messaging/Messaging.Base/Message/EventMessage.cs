using IMessaging;

namespace Messaging.Base {
    public abstract class EventMessage : RequestMessage {
        public bool Forwarded { get; set; }
        public EventMessage() : base(MessageType.Event) { }
        public EventMessage(bool pForwarded): base(MessageType.Event) {
            Forwarded = pForwarded;
        }
    }
}
