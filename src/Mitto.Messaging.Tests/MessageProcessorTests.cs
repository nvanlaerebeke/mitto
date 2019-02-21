using Mitto.IMessaging;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;

namespace Mitto.Messaging.Tests {
	[TestFixture]
	public class MessageProcessorTests {
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
				MessageType.Notification,
				MessageType.Request
			)] MessageType pType,
			[Values(
				false,
				true
			)] bool pTransmitExpected
		) {
			//Arrange
			var objConverter = Substitute.For<IMessageConverter>();
			var objProvider = Substitute.For<IMessageProvider>();

			var objClient = Substitute.For<IQueue.IQueue>();
			var objMessage = Substitute.For<IMessage>();
			var objAction = Substitute.For<IAction>();
			var objResponse = Substitute.For<IResponseMessage>();

			objMessage.Type.Returns(pType);
			objConverter.GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 }))).Returns(objMessage);
			objProvider.GetAction(objClient, objMessage).Returns(objAction);
			objAction.When(a => a.Start()).Do(a => throw new Exception("Some Exception"));

			objConverter.GetResponseMessage(Arg.Is(objMessage), Arg.Is(ResponseCode.Error)).Returns(objResponse);
			objConverter.GetByteArray(Arg.Is(objResponse)).Returns(new byte[] { 1, 2, 3, 4, 5 });

			Mitto.Initialize(new Mitto.Config() {
				MessageConverter = objConverter,
				MessageProvider = objProvider
			});

			//Act
			new MessageProcessor().Process(objClient, new byte[] { 1, 2, 3, 4 });

			//Assert
			objConverter.Received(1).GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })));
			objProvider.Received(1).GetAction(Arg.Is(objClient), Arg.Is(objMessage));

			if (pTransmitExpected) {
				objClient.Received(1).Transmit(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4, 5 })));
			} else {
				objClient.Received(0).Transmit(Arg.Any<byte[]>());
			}
		}

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
			var objProvider = Substitute.For<IMessageProvider>();
			var objClient = Substitute.For<IQueue.IQueue>();
			var objMessage = Substitute.For<IMessage>();
			var objResponse = Substitute.For<IResponseMessage>();
			var objAction = Substitute.For<IAction>();

			objMessage.Type.Returns(MessageType.Request);
			objConverter.GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 }))).Returns(objMessage);
			objProvider.GetAction(objClient, objMessage).Returns(objAction);
			objAction.When(a => a.Start()).Do(a => throw new MessagingException(ResponseCode.Cancelled));


			objResponse.Status.Returns(ResponseCode.Cancelled);
			objConverter.GetResponseMessage(Arg.Is(objMessage), ResponseCode.Cancelled).Returns(objResponse);
			objConverter.GetByteArray(Arg.Is(objResponse)).Returns(new byte[] { 1, 2, 3, 4, 5 });

			Mitto.Initialize(new Mitto.Config() {
				MessageConverter = objConverter,
				MessageProvider = objProvider
			});

			//Act
			var obj = new MessageProcessor();
			obj.Process(objClient, new byte[] { 1, 2, 3, 4 });

			//Assert
			objConverter.Received(1).GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })));
			objConverter.Received(1).GetResponseMessage(Arg.Is<IMessage>(m => m.Equals(objMessage)), ResponseCode.Cancelled);
			objClient.Received(1).Transmit(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4, 5 })));
		}

		/// <summary>
		/// Tests if a message is processed
		/// This means that a call is expected on the IConverter.GetMessage and
		/// that the IMessage of that method is passed to the IRequestManager.Process
		/// </summary>
		[Test, Sequential]
		public void ProcessMessageTest(
			[Values(MessageType.Notification, MessageType.Request)] MessageType pType,
			[Values(false, true)] bool pTransmitExpected
		) {
			//Arrange
			var objConverter = Substitute.For<IMessageConverter>();
			var objClient = Substitute.For<IQueue.IQueue>();
			var objMessage = Substitute.For<IMessage>();
			var objProvider = Substitute.For<IMessageProvider>();
			var objAction = Substitute.For<IAction>();
			var objResponse = Substitute.For<IResponseMessage>();

			objMessage.Type.Returns(pType);
			objConverter.GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 }))).Returns(objMessage);
			objProvider.GetAction(objClient, objMessage).Returns(objAction);
			objAction.Start().Returns(objResponse);
			objConverter.GetByteArray(objResponse).Returns(new byte[] { 1, 2, 3, 4, 5 });

			Mitto.Initialize(new Mitto.Config() {
				MessageConverter = objConverter,
				MessageProvider = objProvider
			});

			//Act
			new MessageProcessor().Process(objClient, new byte[] { 1, 2, 3, 4 });

			//Assert
			objConverter.Received(1).GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })));
			objProvider.Received(1).GetAction(objClient, objMessage);
			objAction.Received(1).Start();
			if (pTransmitExpected) {
				objClient.Received(1).Transmit(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1,2,3,4,5})));
			} else {
				objClient.Received(0).Transmit(Arg.Any<byte[]>());
			}
		}

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
		}


		/// <summary>
		/// Tests if the Response is set on the RequestManager
		/// </summary>
		[Test]
		public void ProcessResponseMessageTest() {
			//Arrange
			var objClient = Substitute.For<IQueue.IQueue>();
			var objConverter = Substitute.For<IMessageConverter>();
			var objRequestManager = Substitute.For<IRequestManager>();
			var objResponse = Substitute.For<IResponseMessage>();

			objResponse.Type.Returns(MessageType.Response);
			objConverter.GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 }))).Returns(objResponse);

			Mitto.Initialize(new Mitto.Config() {
				MessageConverter = objConverter
			});

			//Act
			new MessageProcessor {
				RequestManager = objRequestManager
			}.Process(objClient, new byte[] { 1, 2, 3, 4 });

			//Assert
			objRequestManager.Received(1).SetResponse(Arg.Is<IResponseMessage>(m => m.Equals(objResponse)));
		}
	}
}