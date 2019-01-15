using System;

namespace Mitto.IMessaging {
	public interface IMessage {
		String ID { get; }
		String Name { get; }
		MessageType Type { get; }
		byte GetCode();
	}
}
