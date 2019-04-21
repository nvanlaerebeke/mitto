using System;
using Mitto.IRouting;

namespace Mitto.IMessaging {
	public interface IMessage {
		String ID { get; }
		String Name { get; }
		MessageType Type { get; }
	}
}