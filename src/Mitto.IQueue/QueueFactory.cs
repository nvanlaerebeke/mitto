using Unity;

namespace Mitto.IQueue {
	public static class QueueFactory {
		public static UnityContainer UnityContainer = new UnityContainer();
		public static IQueue Get() {
			return UnityContainer.Resolve<IQueue>();
		}
	}
}