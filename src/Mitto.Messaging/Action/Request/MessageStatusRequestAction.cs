using Mitto.IMessaging;
using Mitto.Messaging.Request;
using Mitto.Messaging.Response;

namespace Mitto.Messaging.Action.Request {
    public class MessageStatusRequestAction : RequestAction<MessageStatusRequest, MessageStatusResponse> {
        public MessageStatusRequestAction(IClient pClient, MessageStatusRequest pMessage) : base(pClient, pMessage) { }

		public override MessageStatusResponse Start() {
			return new MessageStatusResponse(
				Request,
				MessagingFactory.Processor.GetStatus(Request.RequestID)
			);
		}
	}
}