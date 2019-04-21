using Mitto.IRouting;

namespace Mitto.IMessaging {

    public interface IAction { }

    public interface IRequestAction<I, O> : IAction
        where I : IRequestMessage
        where O : IResponseMessage {

        O Start();
    }

    public interface INotificationAction : IAction {

        void Start();
    }
}