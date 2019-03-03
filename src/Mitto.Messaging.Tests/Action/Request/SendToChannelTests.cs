using Mitto.IMessaging;
using Mitto.Messaging.Action.Request;
using NSubstitute;
using NUnit.Framework;

namespace Mitto.Messaging.Tests.Action.Request {
	[TestFixture]
	public class SendToChannelTests {
		/// <summary>
		/// Tests the SendToChannel action for the requests message SendToChannel
		/// This means an Response.ACK message is expected as response and
		/// that the IMessageProvider.GetSubscriptionHandler is called to ge the
		/// subscription handler where the Notify method will be called on 
		/// </summary>
		[Test]
		public void CreateTest() {
			//Arrange
			var objProvider = Substitute.For<IMessageProvider>();
			var objClient = Substitute.For<IClient>();
			var objMessage = Substitute.For<Messaging.Request.ISendToChannel>();
			var objSubscriptionHandler = Substitute.For<Messaging.Action.SubscriptionHandler.IChannel>();

			objMessage.ID.Returns("MyRequestID");
			objMessage.ChannelName.Returns("MyChannel");
			objMessage.Message.Returns("MyMessage");

			objProvider.GetSubscriptionHandler<Messaging.Action.SubscriptionHandler.IChannel>().Returns(objSubscriptionHandler);
			objSubscriptionHandler.Notify(Arg.Is(objClient), Arg.Is(objMessage)).Returns(true);

			Config.Initialize(new Config.ConfigParams() {
				MessageProvider = objProvider
			});

			//Act
			var obj = new SendToChannel(objClient, objMessage);
			var objResponse = obj.Start() as Response.ACK;
			
			//Assert
			Assert.IsNotNull(obj);
			Assert.IsNotNull(objResponse);
			Assert.AreEqual("MyRequestID", objResponse.ID);
			Assert.AreEqual(ResponseCode.Success, objResponse.Status);
			objProvider.Received(1).GetSubscriptionHandler<Messaging.Action.SubscriptionHandler.IChannel>();
			objSubscriptionHandler.Received(1).Notify(Arg.Is(objClient), Arg.Is(objMessage));
		}
	}
}