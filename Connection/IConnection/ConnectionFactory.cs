using Unity;

namespace IConnection {
	public static class ConnectionFactory {
		public static UnityContainer UnityContainer = new UnityContainer();
		public static IServer GetServer() {
			return UnityContainer.Resolve<IServer>();
		}
		public static IClient GetClient() {
			return UnityContainer.Resolve<IClient>();
		}
	}
}
