using System;
using System.Collections.Generic;

namespace IMessaging {
	public interface IMessageProvider {
		List<Type> GetTypes();
	}
}
