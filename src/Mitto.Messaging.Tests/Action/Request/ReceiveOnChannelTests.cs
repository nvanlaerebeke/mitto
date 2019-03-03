using Mitto.IMessaging;
using Mitto.Messaging.Action.Request;
using NSubstitute;
using NUnit.Framework;

namespace Mitto.Messaging.Tests.Action.Request {
	[TestFixture]
	public class ReceiveOnChannelTests {
		/// <summary>
		/// Tests the ReceiveOnChannel action for the requests message ReceiveOnChannel
		/// This means an Response.ACK message is expected as response and
		/// that the ChannelMessageReceived event is called with the channel and message
		/// </summary>
		[Test]
		public void CreateTest() {
			//Arrange
			var objClient = Substitute.For<IClient>();
			var objMessage = Substitute.For<Messaging.Request.ReceiveOnChannelRequest>("MyChannel", "MyMessage");
			var objHandler = Substitute.For<ChannelMessageReceived>();

			objMessage.ID.Returns("MyRequestID");
			//Act
			var obj = new ReceiveOnChannelRequestAction(objClient, objMessage);
			ReceiveOnChannelRequestAction.ChannelMessageReceived += objHandler;
			var objResponse = obj.Start() as Response.ACKResponse;
			
			//Assert
			Assert.IsNotNull(obj);
			Assert.IsNotNull(objResponse);
			Assert.AreEqual("MyRequestID", objResponse.ID);
			objHandler.Received(1).Invoke(Arg.Is("MyChannel"), Arg.Is("MyMessage"));
		}
	}
}