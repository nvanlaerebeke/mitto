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
			objProvider.GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 }))).Returns(objMessage);

			if (pTransmitExpected) {
				objAction = Substitute.For<IRequestAction>();
				objAction.When(a => ((IRequestAction)a).Start()).Do(a => throw new Exception("Some Exception"));
			} else {
				objAction = Substitute.For<INotificationAction>();
				objAction.When(a => ((INotificationAction)a).Start()).Do(a => throw new Exception("Some Exception"));
			}

			objProvider.GetAction(objClient, objMessage).Returns(objAction);
			objProvider.GetResponseMessage(Arg.Is(objMessage), Arg.Is(ResponseCode.Error)).Returns(objResponse);
			objConverter.GetByteArray(Arg.Is(objResponse)).Returns(new byte[] { 1, 2, 3, 4, 5 });

			Config.Initialize(new Config.ConfigParams() {
				MessageConverter = objConverter,
				MessageProvider = objProvider
			});

			//Act
			new MessageProcessor().Process(objClient, new byte[] { 1, 2, 3, 4 });

			//Assert
			objProvider.Received(1).GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })));
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
			var objAction = Substitute.For<IRequestAction>();

			objMessage.Type.Returns(MessageType.Request);
			objProvider.GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 }))).Returns(objMessage);
			objProvider.GetAction(objClient, objMessage).Returns(objAction);
			objAction.When(a => a.Start()).Do(a => throw new MessagingException(ResponseCode.Cancelled));


			objResponse.Status.Returns(ResponseCode.Cancelled);
			objProvider.GetResponseMessage(Arg.Is(objMessage), ResponseCode.Cancelled).Returns(objResponse);
			objConverter.GetByteArray(Arg.Is(objResponse)).Returns(new byte[] { 1, 2, 3, 4, 5 });

			Config.Initialize(new Config.ConfigParams() {
				MessageConverter = objConverter,
				MessageProvider = objProvider
			});

			//Act
			var obj = new MessageProcessor();
			obj.Process(objClient, new byte[] { 1, 2, 3, 4 });

			//Assert
			objProvider.Received(1).GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })));
			objProvider.Received(1).GetResponseMessage(Arg.Is<IMessage>(m => m.Equals(objMessage)), ResponseCode.Cancelled);
			objClient.Received(1).Transmit(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4, 5 })));
		}

		/// <summary>
		/// Tests if a message is processed
		/// This means that a call is expected on the IConverter.GetMessage,
		/// that the Action is requested from the IConverter.GetAction and the IQueue.IQueue
		/// transmit method is called with the IResponseMessage byte[] from the action result
		/// </summary>
		[Test]
		public void ProcessRequestMessageTest() {
			//Arrange
			var objConverter = Substitute.For<IMessageConverter>();
			var objClient = Substitute.For<IQueue.IQueue>();
			var objMessage = Substitute.For<IMessage>();
			var objProvider = Substitute.For<IMessageProvider>();
			var objAction = Substitute.For<IRequestAction>();
			var objResponse = Substitute.For<IResponseMessage>();

			objMessage.Type.Returns(MessageType.Request);
			objProvider.GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 }))).Returns(objMessage);
			objProvider.GetAction(objClient, objMessage).Returns(objAction);
			objProvider.GetByteArray(Arg.Is(objResponse)).Returns(new byte[] { 1, 2, 3, 4, 5 });
			objAction.Start().Returns(objResponse);

			Config.Initialize(new Config.ConfigParams() {
				MessageConverter = objConverter,
				MessageProvider = objProvider
			});

			//Act
			new MessageProcessor().Process(objClient, new byte[] { 1, 2, 3, 4 });

			//Assert
			objProvider.Received(1).GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })));
			objProvider.Received(1).GetAction(objClient, objMessage);
			objAction.Received(1).Start();
			objClient.Received(1).Transmit(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4, 5 })));
		}

		[Test]
		public void ProcessNotificationMessageTest() {
			//Arrange
			var objConverter = Substitute.For<IMessageConverter>();
			var objClient = Substitute.For<IQueue.IQueue>();
			var objMessage = Substitute.For<IMessage>();
			var objProvider = Substitute.For<IMessageProvider>();
			var objAction = Substitute.For<INotificationAction>();
			var objResponse = Substitute.For<IResponseMessage>();

			objMessage.Type.Returns(MessageType.Notification);
			objProvider.GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 }))).Returns(objMessage);
			objProvider.GetAction(objClient, objMessage).Returns(objAction);
			objConverter.GetByteArray(objResponse).Returns(new byte[] { 1, 2, 3, 4, 5 });

			Config.Initialize(new Config.ConfigParams() {
				MessageConverter = objConverter,
				MessageProvider = objProvider
			});

			//Act
			new MessageProcessor().Process(objClient, new byte[] { 1, 2, 3, 4 });

			//Assert
			objProvider.Received(1).GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })));
			objProvider.Received(1).GetAction(objClient, objMessage);
			objAction.Received(1).Start();
			objClient.Received(0).Transmit(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4, 5 })));
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
			var objProvider = Substitute.For<IMessageProvider>();
			var objRequestManager = Substitute.For<IRequestManager>();
			var objQueue = Substitute.For<IQueue.IQueue>();
			var objMessage = Substitute.For<IMessage>();

			objProvider.GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 }))).Returns(m => null);

			Config.Initialize(new Config.ConfigParams() {
				MessageConverter = objConverter,
				MessageProvider = objProvider
			});

			//Act
			new MessageProcessor(objRequestManager).Process(objQueue, new byte[] { 1, 2, 3, 4 });

			//Assert
			objProvider.Received(1).GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })));
			objRequestManager.Received(0).SetResponse(Arg.Any<IResponseMessage>());
			objProvider.Received(0).GetAction(Arg.Any<IQueue.IQueue>(), Arg.Any<IMessage>());
		}


		/// <summary>
		/// Tests if the Response is set on the RequestManager
		/// </summary>
		[Test]
		public void ProcessResponseMessageTest() {
			//Arrange
			var objClient = Substitute.For<IQueue.IQueue>();
			var objProvider = Substitute.For<IMessageProvider>();
			var objRequestManager = Substitute.For<IRequestManager>();
			var objResponse = Substitute.For<IResponseMessage>();

			objResponse.Type.Returns(MessageType.Response);
			objProvider.GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 }))).Returns(objResponse);

			Config.Initialize(new Config.ConfigParams() {
				MessageProvider = objProvider
			});

			//Act
			new MessageProcessor(objRequestManager).Process(objClient, new byte[] { 1, 2, 3, 4 });

			//Assert
			objRequestManager.Received(1).SetResponse(Arg.Is<IResponseMessage>(m => m.Equals(objResponse)));
		}

		/// <summary>
		/// Tests the Request method
		/// This means that the Request message is passed to the RequestManager
		/// </summary>
		[Test]
		public void RequestTest() {
			//Arrange
			var objClient = Substitute.For<IQueue.IQueue>();
			var objMessage = Substitute.For<IMessage>();
			var objAction = Substitute.For<Action<IResponseMessage>>();
			var objRequestManager = Substitute.For<IRequestManager>();

			//Act
			new MessageProcessor(objRequestManager).Request(objClient, objMessage, objAction);

			//Assert
			objRequestManager.Received(1).Request(Arg.Is(objClient), Arg.Is(objMessage), Arg.Is(objAction));
		}
	}
}