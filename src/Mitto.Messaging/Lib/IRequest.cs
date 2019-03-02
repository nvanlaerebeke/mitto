using Mitto.IMessaging;
using System;

namespace Mitto.Messaging {
	public interface IRequest {
		event EventHandler<IRequest> RequestTimedOut;
		IRequestMessage Message { get; }
		void SetResponse(IResponseMessage pResponse);
		void Transmit();
	}
}