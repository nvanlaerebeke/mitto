using Mitto.Logging;
using Mitto.Connection.Websocket;
using Mitto.IConnection;
using Mitto.IMessaging;
using Mitto.IRouting;
using Mitto.Messaging;
using Mitto.Messaging.Json;
using Mitto.Routing.PassThrough;
using Mitto.Subscription.Messaging;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Mitto {

    /// <summary>
    /// ToDo: Move MessageFactory.Provider.LoadTypes from constructor to initialization function
    ///       This is to make sure all needed assemblies are loaded before Mitto starts to load
    ///       the types from the known assemblies
    /// </summary>
    public static class Config {
        private static ILog Log {
            get { return LoggingFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType); }
        }

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
            LoggingFactory.Initialize(pConfig.Logger);
            SubscriptionFactory.Initialize();
            RouterFactory.Initialize(pConfig.RouterProvider);
            ConnectionFactory.Initialize(pConfig.ConnectionProvider);
            MessagingFactory.Initialize(pConfig.MessageProvider, pConfig.MessageConverter, pConfig.MessageProcessor);

            MessagingFactory.Provider.Load(pConfig.Assemblies);
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
            public ILog Logger { get; set; } = new ConsoleLog();
            public IRouterProvider RouterProvider { get; set; } = new RouterProvider();
            public IConnectionProvider ConnectionProvider { get; set; } = new WebSocketConnectionProvider();
            public IMessageConverter MessageConverter { get; set; } = new MessageConverter();
            public IMessageProvider MessageProvider { get; set; } = new MessageProvider();
            public IMessageProcessor MessageProcessor { get; set; } = new MessageProcessor();
            public IEnumerable<string> Assemblies { get; set; } = new List<string>();
        }
    }
}