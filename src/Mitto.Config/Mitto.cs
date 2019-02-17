namespace Mitto {
	public static class Mitto {
		public static void Initialize() => Initialize(new Config());

		public static void Initialize(Config pConfig) {
			IQueue.QueueFactory.Initialize(pConfig.QueueProvider);
			IConnection.ConnectionFactory.Initialize(pConfig.ConnectionProvider);
			IMessaging.MessagingFactory.Initialize(pConfig.MessageProvider, pConfig.MessageCreator, pConfig.MessageProcessor);
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
			public IMessaging.IMessageCreator MessageCreator { get; set; } = new Messaging.Json.MessageCreator();
			public IMessaging.IMessageProvider MessageProvider { get; set; } = new Messaging.MessageProvider();
			public IMessaging.IMessageProcessor MessageProcessor { get; set; } = new Messaging.MessageProcessor();
		}
	}
}