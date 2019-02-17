namespace Mitto.IMessaging {
	/// <summary>
	/// MessageFactrory is responsible for providing the instances for the classes that 
	/// control the messaging.
	/// 
	/// They can be set manually or by using mitto initialization method
	/// </summary>
	public static class MessagingFactory {
		public static void Initialize(IMessageProvider pProvider, IMessageCreator pCreator, IMessageProcessor pProcessor) {
			Provider = pProvider;
			Creator = pCreator;
			Processor = pProcessor;
		}

		public static IMessageProvider Provider { get; set; }
		public static IMessageCreator Creator { get; set; }
		public static IMessageProcessor Processor { get; set; }
	}
}