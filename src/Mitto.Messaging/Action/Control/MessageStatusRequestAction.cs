using Mitto.IMessaging;
using Mitto.Messaging.Control;
using Mitto.Messaging.Response;

namespace Mitto.Messaging.Action.Control {
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