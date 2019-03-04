namespace Mitto.IMessaging {
	public interface IAction { }

	public interface IRequestAction<O> : IAction where O: IResponseMessage {
		O Start();
	}
	public interface INotificationAction : IAction {
		void Start();
	}
}