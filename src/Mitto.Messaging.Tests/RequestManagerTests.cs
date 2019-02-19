using NUnit.Framework;
using NSubstitute;
using System;
using System.Linq;
using Mitto.IMessaging;

namespace Mitto.Messaging.Tests {
	[TestFixture]
	public class RequestManagerTests {
		/// <summary>
		/// Testing what happens when an action raises a MessageException
		/// This means that the Action is created by the IMessageProvider and
		/// the action quits with a MessageException where the status code is used
		/// to return the response message
		/// </summary>
		[Test]
		public void ProcessMessageExceptionTest() {
			//Arrange
			var objConverter = Substitute.For<IMessageConverter>();
			var objClient = Substitute.For<IQueue.IQueue>();
			var objMessage = Substitute.For<IMessage>();
			var objResponse = Substitute.For<IResponseMessage>();

			objResponse.Status.Returns(ResponseCode.Cancelled);
			objConverter.GetResponseMessage(Arg.Is(objMessage), ResponseCode.Cancelled).Returns(objResponse);
			objConverter.GetByteArray(Arg.Is(objResponse)).Returns(new byte[] { 1, 2, 3, 4, 5 });

			Mitto.Initialize(new Mitto.Config() {
				MessageConverter = objConverter
			});

			//Act
			var obj = new RequestManager();
			obj.Process(objClient, objMessage); 

			//Assert
			objConverter.Received(1).GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })));
			objConverter.Received(1).GetResponseMessage(Arg.Is<IMessage>(m => m.Equals(objMessage)), ResponseCode.Cancelled);
			objClient.Received(1).Transmit(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4, 5 })));

			Assert.Inconclusive("Need to remove the reflection stuff");
		}

		/// <summary>
		/// Tests if a message is processed
		/// This means that a call is expected on the IConverter.GetMessage and
		/// that the IMessage of that method is passed to the IRequestManager.Process
		/// </summary>
		[Test]
		public void ProcessMessageTest() {
			//Arrange
			var objConverter = Substitute.For<IMessageConverter>();
			var objRequestManager = Substitute.For<IRequestManager>();
			var objClient = Substitute.For<IQueue.IQueue>();
			var objMessage = Substitute.For<IMessage>();

			objConverter.GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 }))).Returns(objMessage);

			Mitto.Initialize(new Mitto.Config() {
				MessageConverter = objConverter
			});

			//Act
			new MessageProcessor {
				RequestManager = objRequestManager
			}.Process(objClient, new byte[] { 1, 2, 3, 4 });

			//Assert
			objConverter.Received(1).GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })));
			objRequestManager.Received(1).Process(Arg.Is(objClient), Arg.Is(objMessage));
			objClient.Received(0).Transmit(Arg.Any<byte[]>());

			Assert.Inconclusive("Need to remove the reflection stuff");
		}

		/// <summary>
		/// Tests if an unknown exception is handled correctly
		/// This means that process is called with an IMessage and raises an exception
		/// Based on the messagetype a responsemessage(Transmit) is expected
		/// </summary>
		/// <param name="pType"></param>
		/// <param name="pTransmitExpected"></param>
		[Test, Sequential]
		public void ProcessUnknownExceptionTest(
			[Values(
				MessageType.Event,
				MessageType.Notification,
				MessageType.Request,
				MessageType.Response,
				MessageType.Subscribe,
				MessageType.UnSubscribe
			)] MessageType pType,
			[Values(
				true,
				false,
				true,
				false,
				true,
				true
			)] bool pTransmitExpected
		) {
			//Arrange
			var objConverter = Substitute.For<IMessageConverter>();
			var objRequestManager = Substitute.For<IRequestManager>();
			var objClient = Substitute.For<IQueue.IQueue>();
			var objMessage = Substitute.For<IMessage>();
			var objResponse = Substitute.For<IResponseMessage>();

			objMessage.Type.Returns(pType);
			objResponse.Status.Returns(ResponseCode.Cancelled);
			objConverter.GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 }))).Returns(objMessage);
			objConverter.GetResponseMessage(Arg.Is(objMessage), Arg.Is(ResponseCode.Error)).Returns(objResponse);
			objConverter.GetByteArray(Arg.Is(objResponse)).Returns(new byte[] { 1, 2, 3, 4, 5 });
			objRequestManager.When(m => m.Process(Arg.Is(objClient), Arg.Is(objMessage))).Do(m => throw new Exception("Some Exception"));

			Mitto.Initialize(new Mitto.Config() {
				MessageConverter = objConverter
			});

			//Act
			new MessageProcessor {
				RequestManager = objRequestManager
			}.Process(objClient, new byte[] { 1, 2, 3, 4 });

			//Assert
			objConverter.Received(1).GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })));
			if (pTransmitExpected) {
				objClient.Received(1).Transmit(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4, 5 })));
			} else {
				objClient.Received(0).Transmit(Arg.Any<byte[]>());
			}

			Assert.Inconclusive("Need to remove the reflection stuff");
		}
	}
}