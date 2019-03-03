using Mitto.IQueue;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleToAttribute("Mitto.Queue.PassThrough.Tests")]
namespace Mitto.Queue.PassThrough {
	/// <summary>
	/// PassThrought Message Handler
	/// Used for handling the messages by the process itself
	/// 
	/// Ideal for small applications where scaling is less of an issue
	/// or applications where the messages actions are light weight
	/// 
	/// As the name describes, this takes in byte[] data and passes it to the 
	/// IMessageProcessor that will process the message for us
	/// </summary>
	public class Passthrough : IQueue.IQueue {
		public event DataHandler Rx;
		private IQueue.IQueue Queue { get; set; }

		public string ID { get; } =  Guid.NewGuid().ToString();

		internal Passthrough(IQueue.IQueue pInternalQueue) {
			Queue = pInternalQueue;
		}

		public Passthrough() {
			Queue = new InternalQueue(this);
		}

		public void Receive(byte[] pData) {
			Rx?.Invoke(pData);
		}

		/// <summary>
		/// Response/Transmit from Message Handler -> Client (Rx)
		/// </summary>
		/// <param name="pMessage"></param>
		public void Transmit(byte[] pData) {
			//Here we take in a byte[] from the IConnection and must pass an IQueue that 
			//handles the internal Tx/Rx, while this Queue handles the external Tx/Rx
			Queue.Receive(pData);
		}
	}
}