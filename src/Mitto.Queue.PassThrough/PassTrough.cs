using Mitto.IQueue;

namespace Mitto.Queue.PassThrough {
	/// <summary>
	/// PassThrought Message Handler
	/// Used for handling the messages by the process itself
	/// 
	/// Ideal for applications with low message thoughput and for testing
	/// 
	/// As the name describes, this takes in a message (client id + byte payload) and passes it into the processor
	/// </summary>
	public class PassThrough : IQueue.IQueue {
		IQueue.IQueue _objInternalQueue;

		public event DataHandler Rx;

		private IQueue.IQueue Queue {
			get {
				if(_objInternalQueue == null) {
					_objInternalQueue = new InternalQueue(this);
				}
				return _objInternalQueue;
			}
		}

		public PassThrough() { }

		public void Receive(Message pMessage) {
			Rx?.Invoke(pMessage);
		}

		/// <summary>
		/// Response/Transmit from Message Handler -> Client (Rx)
		/// </summary>
		/// <param name="pMessage"></param>
		public void Transmit(Message pMessage) {
			//Here we take in a msg from the IConnection and must pass an IQueue where we also read the Transmit
			Queue.Receive(pMessage);
		}
	}
}