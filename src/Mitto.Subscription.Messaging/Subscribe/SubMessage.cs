using Mitto.IRouting;
using Mitto.Messaging;
using Mitto.Subscription.IMessaging;

namespace Mitto.Subscription.Messaging {

    public abstract class SubMessage : RequestMessage, ISubMessage {

        public SubMessage() : this(MessageType.Request) {
        }

        public SubMessage(MessageType pType) : base(pType) {
        }
    }
}