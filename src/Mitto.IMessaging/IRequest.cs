using System;

namespace Mitto.IMessaging {
	public interface IRequest {
		event EventHandler<IRequest> RequestTimedOut;
		IRequestMessage Message { get; }
		void SetResponse(IResponseMessage pResponse);
		void Transmit();
	}
}