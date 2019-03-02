using Mitto.IMessaging;

namespace Mitto.Messaging.Action.Request {
    public class MessageStatus : RequestAction<Messaging.Request.MessageStatus> {
        public MessageStatus(IClient pClient, Messaging.Request.MessageStatus pMessage) : base(pClient, pMessage) { }

		public override IResponseMessage Start() {
			return new Response.MessageStatus(
				Request,
				MessagingFactory.Processor.GetStatus(Request.RequestID)
			);
		}
	}
}