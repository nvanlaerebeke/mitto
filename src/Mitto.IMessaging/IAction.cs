namespace Mitto.IMessaging {
	public interface IAction {}

	public interface IRequestAction : IAction {
		IResponseMessage Start();
	}
	public interface INotificationAction : IAction {
		void Start();
	}
}