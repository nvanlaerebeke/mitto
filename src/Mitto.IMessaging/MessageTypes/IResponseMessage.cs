using System;

namespace Mitto.IMessaging {
	public interface IResponseMessage : IMessage {
		ResponseStatus Status { get; }
		DateTime StartTime { get; }
		DateTime EndTime { get; }
	}
}