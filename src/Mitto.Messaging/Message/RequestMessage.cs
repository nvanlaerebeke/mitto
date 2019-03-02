using Mitto.IMessaging;
using System;

namespace Mitto.Messaging {
    public abstract class RequestMessage : Message, IRequestMessage {
        public RequestMessage(): base(MessageType.Request, Guid.NewGuid().ToString()) { }
        public RequestMessage(MessageType pType) : base(pType, Guid.NewGuid().ToString()) { }
    }
}
