using Mitto.IQueue;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleToAttribute("Mitto.Queue.PassThrough.Tests")]
namespace Mitto.Queue.PassThrough {
	/// <summary>
	/// PassThrought Message Handler
	/// Used for handling the messages by the process itself
	/// 
	/// Ideal for applications with low message thoughput and for testing
	/// 
	/// As the name describes, this takes in a message (client id + byte payload) and passes it into the processor
	/// 
	/// ToDo: Do we need a reference to IMessaging, arn't we just interested in IQueue.Message and the binary data in it?
	/// </summary>
	public class Passthrough : IQueue.IQueue {
		public event DataHandler Rx;

		internal IQueue.IQueue Queue { get; set; }

		public Passthrough() {
			Queue = new InternalQueue(this);
		}

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