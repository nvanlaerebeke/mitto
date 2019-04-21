using System;

namespace Mitto.IMessaging {
	public interface IRequestMessage: IMessage {
		DateTime StartTime { get; }
	}
}