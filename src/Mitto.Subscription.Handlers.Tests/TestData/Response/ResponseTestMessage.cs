using Mitto.IMessaging;
using Mitto.Messaging;

namespace Mitto.Subscription.Messaging.Tests.TestData.Response {

    public class ResponseTestMessage : ResponseMessage {

        public ResponseTestMessage() {
        }

        public ResponseTestMessage(IRequestMessage pMessage, ResponseStatus pStatus) : base(pMessage, pStatus) {
        }
    }
}