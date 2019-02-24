namespace Mitto {
	public static class Mitto {
		/// <summary>
		/// Initialize Mitto with the default configuration
		/// </summary>
		public static void Initialize() => Initialize(new Config());
		
		/// <summary>
		/// Initialize Mitto with the given configuration
		/// </summary>
		/// <param name="pConfig"></param>
		public static void Initialize(Config pConfig) {
			IQueue.QueueFactory.Initialize(pConfig.QueueProvider);
			IConnection.ConnectionFactory.Initialize(pConfig.ConnectionProvider);
			IMessaging.MessagingFactory.Initialize(pConfig.MessageProvider, pConfig.MessageConverter, pConfig.MessageProcessor);
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
		public class Config {
			public IQueue.IQueueProvider QueueProvider { get; set; } = new Queue.PassThrough.QueueProvider();
			public IConnection.IConnectionProvider ConnectionProvider { get; set; } = new Connection.Websocket.ConnectionProvider();
			public IMessaging.IMessageConverter MessageConverter { get; set; } = new Messaging.Json.MessageConverter();
			public IMessaging.IMessageProvider MessageProvider { get; set; } = new Messaging.MessageProvider();
			public IMessaging.IMessageProcessor MessageProcessor { get; set; } = new Messaging.MessageProcessor();
		}
	}
}