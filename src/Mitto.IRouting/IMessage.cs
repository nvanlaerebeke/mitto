using System;

namespace Mitto.IRouting {
	public interface IMessage {
		String ID { get; }
		String Name { get; }
		MessageType Type { get; }
	}
}
