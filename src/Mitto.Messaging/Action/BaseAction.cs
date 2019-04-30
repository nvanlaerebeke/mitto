using Mitto.IMessaging;
using Mitto.Logging;

namespace Mitto.Messaging.Action {

    /// <summary>
    /// ToDo: Improve the inheritance of IAction/INotificationAction and IRequestAction
    /// IAction should have an internal? Execute method and the INotification & IRequestAction
    /// their own Start method
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseAction<T> : IAction where T : IMessage {
        protected readonly ILog Log = LoggingFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string RequestID => Request.ID;
        public T Request { get; private set; }

        protected IClient Client { private set; get; }

        public BaseAction(IClient pClient, T pRequest) {
            Client = pClient;
            Request = pRequest;
        }
    }
}