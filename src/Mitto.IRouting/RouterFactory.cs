using Mitto.IConnection;

namespace Mitto.IRouting {

    public static class RouterFactory {
        public static IRouterProvider Provider { get; set; }

        public static IRouter Create(IClientConnection pConnection) {
            return Provider.Create(pConnection);
        }

        public static void Initialize(IRouterProvider pProvider) {
            Provider = pProvider;
        }
    }
}