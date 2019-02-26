﻿using Mitto.IMessaging;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;

namespace Mitto.Messaging.Tests {
	[TestFixture]
	public class MessageProcessorTests {

		/// <summary>
		/// Tests if the MessageTypes
		/// </summary>
		/// <param name="pMessageType"></param>
		[Test, Sequential]
		public void ProcessActionMessage(
			[Values(MessageType.Notification, MessageType.Request, MessageType.Response)] MessageType pMessageType,
			[Values(true, true, false)] bool pHasAction
		) {
			//Arrange
			var objClient = Substitute.For<IQueue.IQueue>();
			var objProvider = Substitute.For<IMessageProvider>();
			var objRequestManager = Substitute.For<IRequestManager>();
			var objActionManager = Substitute.For<IActionManager>();
			var objMessage = Substitute.For<IMessage>();
			var objAction = Substitute.For<IAction>();

			objMessage.Type.Returns(pMessageType);
			objProvider.GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 }))).Returns(objMessage);
			objProvider.GetAction(Arg.Is(objClient), Arg.Is(objMessage)).Returns(objAction);

			Config.Initialize(new Config.ConfigParams() {
				MessageProvider = objProvider
			});

			//Act
			new MessageProcessor(objRequestManager, objActionManager).Process(objClient, new byte[] { 1, 2, 3, 4 });

			//Assert
			objProvider.Received(1).GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })));
			if (pHasAction) {
				objProvider.Received(1).GetAction(Arg.Is(objClient), Arg.Is(objMessage));
				objActionManager.Received(1).RunAction(Arg.Is(objClient), Arg.Is(objMessage), Arg.Is(objAction));
			} else {
				objProvider.Received(0).GetAction(Arg.Any<IQueue.IQueue>(), Arg.Any<IMessage>());
				objActionManager.Received(0).RunAction(Arg.Any<IQueue.IQueue>(), Arg.Any<IMessage>(), Arg.Any<IAction>());
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
			var objProvider = Substitute.For<IMessageProvider>();
			var objRequestManager = Substitute.For<IRequestManager>();
			var objActionManager = Substitute.For<IActionManager>();
			var objClient = Substitute.For<IQueue.IQueue>();
			var objMessage = Substitute.For<IMessage>();

			objProvider.GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 }))).Returns(m => null);

			Config.Initialize(new Config.ConfigParams() {
				MessageProvider = objProvider
			});

			//Act
			new MessageProcessor(objRequestManager, objActionManager).Process(objClient, new byte[] { 1, 2, 3, 4 });

			//Assert
			objProvider.Received(1).GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })));
			objRequestManager.Received(0).SetResponse(Arg.Any<IResponseMessage>());
			objProvider.Received(0).GetAction(Arg.Any<IQueue.IQueue>(), Arg.Any<IMessage>());
			objActionManager.Received(0).RunAction(Arg.Any<IQueue.IQueue>(), Arg.Any<IMessage>(), Arg.Any<IAction>());
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
			var objActionManager = Substitute.For<IActionManager>();
			var objResponse = Substitute.For<IResponseMessage>();

			objResponse.Type.Returns(MessageType.Response);
			objProvider.GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 }))).Returns(objResponse);

			Config.Initialize(new Config.ConfigParams() {
				MessageProvider = objProvider
			});

			//Act
			new MessageProcessor(objRequestManager, objActionManager).Process(objClient, new byte[] { 1, 2, 3, 4 });

			//Assert
			objRequestManager.Received(1).SetResponse(Arg.Is<IResponseMessage>(m => m.Equals(objResponse)));
			objProvider.Received(0).GetAction(Arg.Any<IQueue.IQueue>(), Arg.Any<IMessage>());
			objActionManager.Received(0).RunAction(Arg.Any<IQueue.IQueue>(), Arg.Any<IMessage>(), Arg.Any<IAction>());
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
			var objActionManager = Substitute.For<IActionManager>();

			//Act
			new MessageProcessor(objRequestManager, objActionManager).Request(objClient, objMessage, objAction);

			//Assert
			objRequestManager.Received(1).Request(Arg.Is(objClient), Arg.Is(objMessage), Arg.Is(objAction));
		}
	}
}