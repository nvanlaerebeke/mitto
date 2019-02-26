using NUnit.Framework;
using NSubstitute;
using System;
using System.Linq;
using Mitto.IMessaging;

namespace Mitto.Messaging.Tests {
	[TestFixture]
	public class ActionManagerTests {

		/// <summary>
		/// Test if when passing null as IAction 
		/// This test if no exceptions are raised
		/// </summary>
		[Test]
		public void NullActionTest() {
			//Arrange
			var objClient = Substitute.For<IQueue.IQueue>();
			var objMessage = Substitute.For<IMessage>();
			var objAction = Substitute.For<IRequestAction>();
			objMessage.Type.Returns(MessageType.Request);
			//Act
			new ActionManager().RunAction(objClient, objMessage, null);
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
				MessageType.Notification,
				MessageType.Request
			)] MessageType pType,
			[Values(
				false,
				true
			)] bool pTransmitExpected
		) {
			//Arrange
			var objProvider = Substitute.For<IMessageProvider>();

			var objClient = Substitute.For<IQueue.IQueue>();
			var objMessage = Substitute.For<IMessage>();
			var objAction = Substitute.For<IAction>();
			var objResponse = Substitute.For<IResponseMessage>();

			objMessage.Type.Returns(pType);

			if (pTransmitExpected) {
				objAction = Substitute.For<IRequestAction>();
				objAction.When(a => ((IRequestAction)a).Start()).Do(a => throw new Exception("Some Exception"));
			} else {
				objAction = Substitute.For<INotificationAction>();
				objAction.When(a => ((INotificationAction)a).Start()).Do(a => throw new Exception("Some Exception"));
			}
			objProvider.GetResponseMessage(Arg.Is(objMessage), Arg.Is(ResponseCode.Error)).Returns(objResponse);
			objProvider.GetByteArray(Arg.Is(objResponse)).Returns(new byte[] { 1, 2, 3, 4, 5 });

			Config.Initialize(new Config.ConfigParams() {
				MessageProvider = objProvider
			});

			//Act
			new ActionManager().RunAction(objClient, objMessage, objAction);

			System.Threading.Thread.Sleep(50);

			//Assert
			if (pTransmitExpected) {
				((IRequestAction)objAction).Received(1).Start();
				objProvider.Received(1).GetResponseMessage(Arg.Is(objMessage), ResponseCode.Error);
				objClient.Received(1).Transmit(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4, 5 })));
			} else {
				((INotificationAction)objAction).Received(1).Start();
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
			var objProvider = Substitute.For<IMessageProvider>();

			var objClient = Substitute.For<IQueue.IQueue>();
			var objMessage = Substitute.For<IMessage>();
			var objAction = Substitute.For<IRequestAction>();

			var objResponse = Substitute.For<IResponseMessage>();

			objMessage.Type.Returns(MessageType.Request);
			objAction.When(a => a.Start()).Do(a => throw new MessagingException(ResponseCode.Cancelled));

			objProvider.GetResponseMessage(Arg.Is(objMessage), ResponseCode.Cancelled).Returns(objResponse);
			objProvider.GetByteArray(Arg.Is(objResponse)).Returns(new byte[] { 1, 2, 3, 4, 5 });

			Config.Initialize(new Config.ConfigParams() {
				MessageProvider = objProvider
			});

			//Act
			new ActionManager().RunAction(objClient, objMessage, objAction);

			System.Threading.Thread.Sleep(50);

			//Assert
			objAction.Received(1).Start();
			objProvider.Received(1).GetResponseMessage(Arg.Is(objMessage), ResponseCode.Cancelled);
			objProvider.Received(1).GetByteArray(Arg.Is(objResponse));
			objClient.Received(1).Transmit(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4, 5 })));
		}

		/// <summary>
		/// Tests handling a Notification Action
		/// This means that the NotificationAction.Start() is called and no Transmit is never called
		/// </summary>
		[Test]
		public void ProcessNotificationMessageTest() {
			//Arrange
			var objClient = Substitute.For<IQueue.IQueue>();
			var objMessage = Substitute.For<IMessage>();
			var objAction = Substitute.For<INotificationAction>();

			var objProvider = Substitute.For<IMessageProvider>();

			objMessage.Type.Returns(MessageType.Notification);
			objProvider.GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 }))).Returns(objMessage);
			objProvider.GetAction(objClient, objMessage).Returns(objAction);

			Config.Initialize(new Config.ConfigParams() {
				MessageProvider = objProvider
			});

			//Act
			new ActionManager().RunAction(objClient, objMessage, objAction);

			System.Threading.Thread.Sleep(50);

			//Assert
			objAction.Received(1).Start();
			objClient.Received(0).Transmit(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4, 5 })));
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
			var objClient = Substitute.For<IQueue.IQueue>();
			var objMessage = Substitute.For<IMessage>();
			var objAction = Substitute.For<IRequestAction>();

			var objProvider = Substitute.For<IMessageProvider>();
			var objResponse = Substitute.For<IResponseMessage>();

			objMessage.Type.Returns(MessageType.Request);
			objProvider.GetByteArray(Arg.Is(objResponse)).Returns(new byte[] { 1, 2, 3, 4, 5 });
			objAction.Start().Returns(objResponse);

			Config.Initialize(new Config.ConfigParams() {
				MessageProvider = objProvider
			});

			//Act
			new ActionManager().RunAction(objClient, objMessage, objAction);

			System.Threading.Thread.Sleep(50);

			//Assert
			objAction.Received(1).Start();
			objProvider.Received(1).GetByteArray(Arg.Is(objResponse));
			objClient.Received(1).Transmit(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4, 5 })));
		}

		/// <summary>
		/// Tests if an Action for a request is still in progress
		/// </summary>
		[Test]
		public void IsAliveTest() {
			//Arrange
			var objMessage = Substitute.For<IMessage>();
			var objAction = Substitute.For<IRequestAction>();

			objMessage.ID.Returns("MyID");
			objMessage.Type.Returns(MessageType.Request);

			Config.Initialize(new Config.ConfigParams() {
				MessageProvider = Substitute.For<IMessageProvider>()
			});

			//Act
			var objActionManager = new ActionManager();
			objActionManager.RunAction(Substitute.For<IQueue.IQueue>(), objMessage, objAction);
			objAction.When(a => a.Start()).Do(a => System.Threading.Thread.Sleep(5000));

			//Assert
			Assert.IsTrue(objActionManager.IsBusy("MyID"));
		}
	}
}