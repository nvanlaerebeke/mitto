using NUnit.Framework;
using NSubstitute;
using Mitto.IQueue;
using System.Linq;
using System;

namespace Mitto.Queue.PassThrough.Tests {
	[TestFixture()]
	public class PassThroughTests {
		[Test()]
		public void ReceiveTest() {
			//Arrange
			var handler = Substitute.For<EventHandler<byte[]>>();
			
			//Act
			var obj = new Passthrough();
			obj.Rx += handler;
			obj.Receive(new byte[] { 0, 1, 2, 4 });

			//Assert
			handler
				.Received(1)
				.Invoke(Arg.Is(obj), Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 0, 1, 2, 4 })))
			;
		}

		[Test()]
		public void TransmitTest() {
			//Arrange
			var objInternalQueue = Substitute.For<IQueue.IQueue>();

			//Act
			new Passthrough(objInternalQueue).Transmit(new byte[] { 1, 2, 3, 4 });

			//Assert
			objInternalQueue.Received(1).Receive(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })));
		}
	}
}