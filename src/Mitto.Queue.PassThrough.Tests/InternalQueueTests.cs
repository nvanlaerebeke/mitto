using NUnit.Framework;
using NSubstitute;
using Mitto.IQueue;
using System.Linq;

namespace Mitto.Queue.PassThrough.Tests {
	[TestFixture()]
	public class InternalQueueTests {

		/// <summary>
		/// Testing the Receive on an internal queue
		/// This means that a message processor is called to process the message
		/// with as parameters the internal queue and transmitted message
		/// </summary>
		[Test()]
		public void ReceiveTest() {
			//mock the messageprocessor
			var objProcessor = Substitute.For<IMessaging.IMessageProcessor>();
			Config.Initialize(new Config.ConfigParams() {
				MessageProcessor = objProcessor
			});

			var obj = new InternalQueue(new Passthrough());
			obj.Receive(new byte[] { 0, 1, 2, 4 });

			//Expecting:
			// - InternalQueue 'obj' created above
			// - the correct message (compare with .Equals)
			objProcessor.Received().Process(
				obj, 
				Arg.Is<byte[]>(b => 
					b.SequenceEqual(new byte[] { 0, 1, 2, 4 })
				)
			);
		}

		/// <summary>
		/// Testing the Transmit of an internal queue
		/// This means receiving it back in the passed queue
		/// </summary>
		[Test()]
		public void TransmitTest() {
			var objQueue = Substitute.For<IQueue.IQueue>();

			var obj = new InternalQueue(objQueue);
			obj.Transmit(new byte[] { 0, 1, 2, 4 });

			objQueue.Received().Receive(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 0, 1, 2, 4 })));
		}
	}
}