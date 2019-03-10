using Mitto.IMessaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mitto.Routing.RabbitMQ.Publisher {
	internal class Request : IRequest {
		public IRequestMessage Message => throw new NotImplementedException();

		public event EventHandler<IRequest> RequestTimedOut;

		public void SetResponse(IResponseMessage pResponse) {
			throw new NotImplementedException();
		}

		public void Transmit() {
			throw new NotImplementedException();
		}
	}

	internal interface IRequest {
		event EventHandler<IRequest> RequestTimedOut;
		IRequestMessage Message { get; }
		void SetResponse(IResponseMessage pResponse);
		void Transmit();
	}
}