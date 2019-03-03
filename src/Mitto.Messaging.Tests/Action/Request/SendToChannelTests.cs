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
			var objMessage = Substitute.For<Messaging.Request.SendToChannel>("MyChannel", "MyMessage");
			var objSubscriptionHandler = Substitute.For<Messaging.Action.SubscriptionHandler.Channel>();

			objMessage.ID.Returns("MyRequestID");
			objProvider.GetSubscriptionHandler<Messaging.Action.SubscriptionHandler.Channel>().Returns(objSubscriptionHandler);

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
			objProvider.Received(1).GetSubscriptionHandler<Messaging.Action.SubscriptionHandler.Channel>();
			objSubscriptionHandler.Received(1).Notify(Arg.Is(objClient), Arg.Is(objMessage));
		}
	}
}