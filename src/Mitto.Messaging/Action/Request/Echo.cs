using Mitto.IMessaging;

namespace Mitto.Messaging.Action.Request {
    public class Echo : RequestAction<Messaging.Request.Echo> {
        public Echo(IClient pClient, Messaging.Request.Echo pMessage) : base(pClient, pMessage) { }

		public override IResponseMessage Start() {
			return new Response.Echo(Request);
		}
	}
}