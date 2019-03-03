using Mitto.IMessaging;
using Mitto.Messaging.Request;
using Mitto.Messaging.Response;

namespace Mitto.Messaging.Action.Request {
    public class EchoRequestAction : RequestAction<EchoRequest, EchoResponse> {
        public EchoRequestAction(IClient pClient, EchoRequest pMessage) : base(pClient, pMessage) { }

		public override IResponseMessage Start() {
			return new EchoResponse(Request);
		}
	}
}