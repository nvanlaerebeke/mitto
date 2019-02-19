using NUnit.Framework;
using NSubstitute;
using Mitto.IQueue;
using System.Linq;

namespace Mitto.Queue.PassThrough.Tests {
	[TestFixture()]
	public class PassThroughTests {
		[Test()]
		public void ReceiveTest() {
			//Arrange
			var handler = Substitute.For<DataHandler>();
			var obj = new Passthrough();
			obj.Rx += handler;

			//Act
			//Do not pass an object to Receive and the .Equals to prevent comparing references
			//and compare the actual content (.Equals)
			obj.Receive(new byte[] { 0, 1, 2, 4 });

			//Assert
			handler
				.Received(1)
				.Invoke(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 0, 1, 2, 4 })))
			;
		}

		[Test()]
		public void TransmitTest() {
			//Arrange
			var objInternalQueue = Substitute.For<IQueue.IQueue>();

			//Act
			new Passthrough {
				Queue = objInternalQueue
			}.Transmit(new byte[] { 1, 2, 3, 4 });

			//Assert
			objInternalQueue.Received(1).Receive(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })));
		}
	}
}