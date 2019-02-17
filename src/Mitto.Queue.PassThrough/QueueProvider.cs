namespace Mitto.Queue.PassThrough {
	public class QueueProvider : IQueue.IQueueProvider {
		public IQueue.IQueue Create() {
			return new Passthrough();
		}
	}
}