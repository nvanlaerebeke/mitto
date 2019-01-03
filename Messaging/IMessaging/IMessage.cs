using System;

namespace IMessaging {
	public interface IMessage {
		String ID { get; }
		String Name { get; }
		MessageType Type { get; }
		byte GetCode();
	}
}
