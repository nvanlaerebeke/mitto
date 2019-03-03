namespace Mitto.IConnection {
	public static class ConnectionFactory {

		private static IConnectionProvider Provider { get; set; }
		public static IServer CreateServer() {
			return Provider.CreateServer();
		}
		public static IClient CreateClient() {
			return Provider.CreateClient();
		}

		public static void Initialize(IConnectionProvider pProvider) {
			Provider = pProvider;
		}
	}
}