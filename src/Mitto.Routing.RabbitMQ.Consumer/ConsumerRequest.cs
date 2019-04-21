using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mitto.IRouting;

namespace Mitto.Routing.RabbitMQ.Consumer {
	internal class ConsumerRequest : IRequest {
		public string ID => Request.RequestID;

		public MessageStatus Status => throw new NotImplementedException();

		public event EventHandler<IRequest> RequestTimedOut;


		private readonly RoutingFrame Request;
		private readonly SenderQueue ProviderQueue;

		public ConsumerRequest(SenderQueue pProviderQueue, RoutingFrame pRequest) {
			ProviderQueue = pProviderQueue;
			Request = pRequest;
		}

		public void Send() {
			Console.WriteLine($"Sending request with ID {ID}");
			new ConsumerRouter(ID, ProviderQueue, Request).Start();
		}

		public void SetResponse(RoutingFrame pFrame) {
			//Not applicable, the MessageRouter will handle it
		}
	}
}
