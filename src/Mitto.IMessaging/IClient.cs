using Mitto.IRouting;
using System;

namespace Mitto.IMessaging {

    public interface IClient : IEquatable<IClient> {

        event EventHandler<IClient> Disconnected;

        string ID { get; }
        IRouter Router { get; }

        void Transmit(IMessage pMessage);

        void Request<R>(IRequestMessage pMessage, Action<R> pAction) where R : IResponseMessage;

        bool IsAlive(string pRequestID);
        void Notify(INotificationMessage pNotifyMessage);
    }
}