using Mitto.IRouting;
using Mitto.Messaging;
using Mitto.Subscription.IMessaging;

namespace Mitto.Subscription.Messaging {

    public abstract class UnSubMessage : RequestMessage, IUnSubMessage {

        public UnSubMessage() : this(MessageType.Request) {
        }

        public UnSubMessage(MessageType pType) : base(pType) {
        }
    }
}