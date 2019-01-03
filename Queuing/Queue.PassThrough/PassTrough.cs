using IQueue;

namespace Queue.PassThrough {
	/// <summary>
	/// PassThrought Message Handler
	/// Used for handling the messages by the process itself
	/// 
	/// Ideal for applications with low message thoughput and for testing
	/// 
	/// As the name describes, this takes in a message (client id + byte payload) and passes it into the processor
	/// </summary>
	public class PassThrough : IQueue.IQueue {
		public event DataHandler Rx;

		/// <summary>
		/// Response/Transmit from Message Handler -> Client (Rx)
		/// </summary>
		/// <param name="pMessage"></param>
		public void Respond(Message pMessage) {
			Rx?.Invoke(pMessage);
		}

		/// <summary>
		/// Transmit data from Client -> Message Handler (Tx)
		/// </summary>
		/// <param name="pMessage"></param>
		public void Transmit(Message pMessage) {
			Messaging.Base.MessageProcessor.Process(this, pMessage);
		}
	}
}
