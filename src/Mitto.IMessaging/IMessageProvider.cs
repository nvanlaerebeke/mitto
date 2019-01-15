using System;
using System.Collections.Generic;

namespace Mitto.IMessaging {
	public interface IMessageProvider {
		List<Type> GetTypes();
	}
}
