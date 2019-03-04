using Mitto.IMessaging;

namespace Mitto.Messaging.Action {
	public abstract class RequestAction<I, O> : BaseAction<I>, IRequestAction<O>
		where I : IRequestMessage
		where O : IResponseMessage {

		public RequestAction(IClient pClient, I pRequest) : base(pClient, pRequest) { }
		public abstract O Start();
	}
}