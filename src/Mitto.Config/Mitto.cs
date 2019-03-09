using Mitto.Connection.Websocket;
using Mitto.IConnection;
using Mitto.IMessaging;
using Mitto.IQueue;
using Mitto.Messaging;
using Mitto.Messaging.Json;
using Mitto.Queue.PassThrough;

namespace Mitto {
	public static class Config {
		/// <summary>
		/// Initialize Mitto with the default configuration
		/// </summary>
		public static void Initialize() => Initialize(new ConfigParams());
		
		/// <summary>
		/// Initialize Mitto with the given configuration
		/// </summary>
		/// <param name="pConfig"></param>
		public static void Initialize(ConfigParams pConfig) {
			QueueFactory.Initialize(pConfig.QueueProvider);
			ConnectionFactory.Initialize(pConfig.ConnectionProvider);
			MessagingFactory.Initialize(pConfig.MessageProvider, pConfig.MessageConverter, pConfig.MessageProcessor);
		}

		/// <summary>
		/// This config class hold the classes that represent the different modules Mitto will use
		/// 
		/// By default the following configuration will be used:
		/// - Passthrough queue 
		/// - Websockets
		/// - Json serialization
		/// - Standard way of processing messages
		/// </summary>
		public class ConfigParams {
			public IQueueProvider QueueProvider { get; set; } = new QueueProvider();
			public IConnectionProvider ConnectionProvider { get; set; } = new WebSocketConnectionProvider();
			public IMessageConverter MessageConverter { get; set; } = new MessageConverter();
			public IMessageProvider MessageProvider { get; set; } = new MessageProvider();
			public IMessageProcessor MessageProcessor { get; set; } = new MessageProcessor();
		}
	}
}