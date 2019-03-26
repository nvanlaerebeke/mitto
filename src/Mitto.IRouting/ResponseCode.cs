/// <summary>
/// ToDo: move back to IMessaging
/// </summary>
namespace Mitto.IRouting {
    public enum ResponseState {
        Success = 0,
        Error = 1,
		Cancelled = 2,
		TimeOut = 3
	}
}