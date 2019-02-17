namespace Mitto.IQueue {
	public static class QueueFactory {
		private static IQueueProvider Provider { get; set; }

		public static IQueue Create() {
			return Provider.Create();
		}

		public static void Initialize(IQueueProvider pProvider) {
			Provider = pProvider;
		}
	}
}