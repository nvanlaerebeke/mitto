using System;

/// <summary>
/// ToDo: move back to IMessaging
/// </summary>
namespace Mitto.IRouting {
	public interface IResponseMessage : IMessage {
		ResponseStatus Status { get; }
		DateTime StartTime { get; }
		DateTime EndTime { get; }
	}
}