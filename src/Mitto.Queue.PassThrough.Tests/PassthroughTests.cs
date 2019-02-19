using NUnit.Framework;
using NSubstitute;
using Mitto.IQueue;
using System.Linq;

namespace Mitto.Queue.PassThrough.Tests {
	[TestFixture()]
	public class PassThroughTests {
		[Test()]
		public void ReceiveTest() {
			var handler = Substitute.For<DataHandler>();
			var obj = new Passthrough();
			obj.Rx += handler;

			//Do not pass an object to Receive and the .Equals to prevent comparing references
			//and compare the actual content (.Equals)
			obj.Receive(new byte[] { 0, 1, 2, 4 });
			handler
				.Received(1)
				.Invoke(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 0, 1, 2, 4 })))
			;
		}

		[Test()]
		public void TransmitTest() {
			var objInternalQueue = Substitute.For<IQueue.IQueue>();

			var obj = new Passthrough();
			obj.Queue = objInternalQueue;
			obj.Transmit(new byte[] { 1, 2, 3, 4 });

			objInternalQueue.Received(1).Receive(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })));
		}
	}
}