using Mitto.IMessaging;
using System;

namespace Mitto.Messaging.Base {
    public abstract class RequestMessage : Message {
        public RequestMessage(): base(MessageType.Request, Guid.NewGuid().ToString()) { }
        public RequestMessage(MessageType pType) : base(pType, Guid.NewGuid().ToString()) { }
    }
}
