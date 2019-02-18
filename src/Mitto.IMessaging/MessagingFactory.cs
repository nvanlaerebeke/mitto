namespace Mitto.IMessaging {
	/// <summary>
	/// MessageFactrory is responsible for providing the instances for the classes that 
	/// control the messaging.
	/// 
	/// They can be set manually or by using mitto initialization method
	/// </summary>
	public static class MessagingFactory {
		public static void Initialize(IMessageProvider pProvider, IMessageConverter pConverter, IMessageProcessor pProcessor) {
			Provider = pProvider;
			Converter = pConverter;
			Processor = pProcessor;
		}

		public static IMessageProvider Provider { get; set; }
		public static IMessageConverter Converter { get; set; }
		public static IMessageProcessor Processor { get; set; }
	}
}