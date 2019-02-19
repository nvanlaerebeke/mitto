using Mitto.IMessaging;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;

namespace Mitto.Messaging.Tests {
	[TestFixture]
	public class MessageProcessorTests {
		/// <summary>
		/// Tests if a none existing message is ignored correctly
		/// This means that RequestManager.Process is never called and 
		/// that no IQueue.Transmit() is done (Response message to an exception
		/// </summary>
		[Test]
		public void ProcessUnknownMessageTest() {
			//Arrange
			var objConverter = Substitute.For<IMessageConverter>();
			var objRequestManager = Substitute.For<IRequestManager>();
			var objQueue = Substitute.For<IQueue.IQueue>();
			var objMessage = Substitute.For<IMessage>();

			objConverter.GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 }))).Returns(m => null);

			Mitto.Initialize(new Mitto.Config() {
				MessageConverter = objConverter
			});

			//Act
			new MessageProcessor {
				RequestManager = objRequestManager
			}.Process(objQueue, new byte[] { 1, 2, 3, 4 });

			//Assert
			objConverter.Received(1).GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })));
			objRequestManager.Received(0).Process(Arg.Any<IQueue.IQueue>(), Arg.Any<IMessage>());
		}
	}
}