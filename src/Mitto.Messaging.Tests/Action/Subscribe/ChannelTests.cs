﻿using Mitto.IMessaging;
using NSubstitute;
using NUnit.Framework;

namespace Mitto.Messaging.Tests.Action.Subscribe {
	[TestFixture]
	public class ChannelTests {
		/// <summary>
		/// Tests the creation of the Channel subscription message
		/// This means that the subscriptionhandler is gotten from the 
		/// IMessageProvider and that the Sub() method on it is called
		/// The response is exepcted to be an ACK message
		/// </summary>
		[Test]
		public void CreateTest() {
			//Arrange
			var objProvider = Substitute.For<IMessageProvider>();
			var objClient = Substitute.For<IClient>();
			var objRequestMessage = Substitute.For<Messaging.Subscribe.Channel>("MyChannel");
			var objSubscriptionHandler = Substitute.For<Messaging.Action.SubscriptionHandler.Channel>();

			objRequestMessage.ID.Returns("MyRequestID");
			objProvider.GetSubscriptionHandler<Messaging.Action.SubscriptionHandler.Channel>().Returns(objSubscriptionHandler);
			//objSubscriptionHandler.Sub(Arg.Is(objClient), Arg.Is(objRequestMessage)).Returns(true);

			Config.Initialize(new Config.ConfigParams() {
				MessageProvider = objProvider
			});

			//Act
			var obj = new Mitto.Messaging.Action.Subscribe.Channel(objClient, objRequestMessage);
			var objResponse = obj.Start() as Messaging.Response.ACK;

			//Assert
			Assert.NotNull(obj);
			Assert.NotNull(objResponse);
			Assert.AreEqual("MyRequestID", objResponse.ID);
			Assert.AreEqual(ResponseCode.Success, objResponse.Status);
			objProvider.Received(1).GetSubscriptionHandler<Messaging.Action.SubscriptionHandler.Channel>();
			objSubscriptionHandler.Received(1).Sub(Arg.Is(objClient), Arg.Is(objRequestMessage));
		}
	}
}