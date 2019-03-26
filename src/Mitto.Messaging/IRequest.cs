using Mitto.IMessaging;
using Mitto.IRouting;
using System;

namespace Mitto.Messaging {
	internal interface IRequest {
		event EventHandler<IRequest> RequestTimedOut;
		IRequestMessage Message { get; }
		void SetResponse(IResponseMessage pResponse);
		void Transmit();
	}
}