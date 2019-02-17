using NUnit.Framework;
using NSubstitute;
using Mitto.IQueue;

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
			obj.Receive(new Message("MyClientID", new byte[] { 0, 1, 2, 4 }));
			handler
				.Received(1)
				.Invoke(Arg.Is<Message>(m => 
					m.Equals(
						new Message("MyClientID", new byte[] { 0, 1, 2, 4 }))
					)
				);
		}

		[Test()]
		public void TransmitTest() {
			var objInternalQueue = Substitute.For<IQueue.IQueue>();

			var obj = new Passthrough();
			obj.Queue = objInternalQueue;
			var objMessage = new IQueue.Message("MyClientID", new byte[] { 0, 1, 2, 4 });
			obj.Transmit(objMessage);

			objInternalQueue.Received(1).Receive(objMessage);
		}
	}
}