/// <summary>
/// ToDo: move back to IMessaging
/// </summary>
namespace Mitto.IMessaging {
    public enum ResponseState {
        Success = 0,
        Error = 1,
		Cancelled = 2,
		TimeOut = 3
	}
}