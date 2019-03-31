using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mitto.Routing.RabbitMQ.Guardian {
	internal class Negotiator {
		private readonly string Prefix;
		public Negotiator(String pQueuePrefix) {
			Prefix = pQueuePrefix;
		}

		public async Task<Guardian> Start() {
			return await Task.Run(() => {
				//ControlFactory.Processor.Request(new )
				return new Guardian();
			});
		}
	}
}
