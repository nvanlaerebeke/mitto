using System;

/// <summary>
/// ToDo: move back to IMessaging
/// </summary>
namespace Mitto.IRouting {
	public interface IRequestMessage: IMessage {
		DateTime StartTime { get; }
	}
}