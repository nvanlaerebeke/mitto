using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mitto.Routing.RabbitMQ.Guardian {
	public class GuardianProvider {
		private Guardian Guardian;

		public async Task<Guardian> GetGuardian() {
			return await Task.Run(() => {
				if(Guardian != null) {
					return Guardian;
				}
				Guardian = new Guardian();
				return Guardian;
			});
		}
	}
}
