using Mitto.IMessaging;

namespace Mitto.Messaging {
    interface IRequestWrapper {
        void Transmit();

        void SetResponse(IResponseMessage pMessage);
    }
}