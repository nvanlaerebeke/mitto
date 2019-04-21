using Mitto.Connection.Websocket;
using Mitto.IConnection;
using Mitto.ILogging;
using Mitto.IMessaging;
using Mitto.IRouting;
using Mitto.Logging;
using Mitto.Messaging;
using Mitto.Messaging.Json;
using Mitto.Routing.PassThrough;
using Mitto.Subscription.Messaging;

namespace Mitto {

    /// <summary>
    /// ToDo: Move MessageFactory.Provider.LoadTypes from constructor to initialization function
    ///       This is to make sure all needed assemblies are loaded before Mitto starts to load
    ///       the types from the known assemblies
    /// </summary>
    public static class Config {

        /// <summary>
        /// Initialize Mitto with the default configuration
        /// </summary>
        public static void Initialize() {
            SubscriptionFactory.Initialize();
            Initialize(new ConfigParams());
        }

        /// <summary>
        /// Initialize Mitto with the given configuration
        /// </summary>
        /// <param name="pConfig"></param>
        public static void Initialize(ConfigParams pConfig) {
            LogFactory.Initialize(pConfig.LogProvider);
            SubscriptionFactory.Initialize();
            RouterFactory.Initialize(pConfig.RouterProvider);
            ConnectionFactory.Initialize(pConfig.ConnectionProvider);
            MessagingFactory.Initialize(pConfig.MessageProvider, pConfig.MessageConverter, pConfig.MessageProcessor);
        }

        /// <summary>
        /// This config class hold the classes that represent the different modules Mitto will use
        ///
        /// By default the following configuration will be used:
        /// - PassThrough queue
        /// - WebSockets
        /// - JSON serialization
        /// - Standard way of processing messages
        /// </summary>
        public class ConfigParams {
            public ILogProvider LogProvider { get; set; } = new LogProvider();
            public IRouterProvider RouterProvider { get; set; } = new RouterProvider();
            public IConnectionProvider ConnectionProvider { get; set; } = new WebSocketConnectionProvider();
            public IMessageConverter MessageConverter { get; set; } = new MessageConverter();
            public IMessageProvider MessageProvider { get; set; } = new MessageProvider();
            public IMessageProcessor MessageProcessor { get; set; } = new MessageProcessor();
        }
    }
}