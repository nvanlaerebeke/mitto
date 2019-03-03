using Mitto.IMessaging;
using NSubstitute;
using NUnit.Framework;

namespace Mitto.Messaging.Tests.Action.SubscriptionHandler {
	[TestFixture]
	public class ChannelTests {
		/// <summary>
		/// Tests the Notify method for multiple subscribed clients
		/// This means that when 2 clients subscribe, that 1 event is 
		/// raised for the target while the source is skipped
		/// </summary>
		[Test]
		public void NotifyTest() {
			//Arrange
			var objClient1 = Substitute.For<IClient>();
			var objClient2 = Substitute.For<IClient>();
			var objSendMessage = Substitute.For<Messaging.Request.SendToChannel>("MyChannel", "MyMessage");
			var objSubMessage = Substitute.For<Messaging.Subscribe.Channel>("MyChannel");

			objClient1.Equals(objClient1).Returns(true);

			//Act
			var obj = new Messaging.Action.SubscriptionHandler.Channel();
			obj.Sub(objClient1, objSubMessage);
			obj.Sub(objClient2, objSubMessage);
			obj.Notify(objClient1, objSendMessage);

			//Assert
			objClient1.Received(0).Transmit(Arg.Any<Messaging.Request.ReceiveOnChannel>());
			objClient2.Received(1).Transmit(Arg.Any<Messaging.Request.ReceiveOnChannel>());
		}

		/// <summary>
		/// ToDo: find a way to test unsubscribe, ISubstitute objects are not removed
		/// </summary>
		[Test]
		public void UnSub() {
			//Arrange
			var objClient1 = Substitute.For<IClient>();
			var objClient2 = Substitute.For<IClient>();
			var objSendMessage = Substitute.For<Messaging.Request.SendToChannel>("MyChannel", "MyMessage");
			var objSubMessage = Substitute.For<Messaging.Subscribe.Channel>("MyChannel");
			var objUnSubMessage = Substitute.For<Messaging.UnSubscribe.Channel>("MyChannel");

			objClient1.ID.Returns("Client1");
			objClient1.Equals(Arg.Is(objClient1)).Returns(true);

			objClient2.ID.Returns("Client2");

			//Act
			var obj = new Messaging.Action.SubscriptionHandler.Channel();
			obj.Sub(objClient1, objSubMessage);
			obj.Sub(objClient2, objSubMessage);
			obj.Notify(objClient1, objSendMessage);

			//Assert
			objClient1.Received(0).Transmit(Arg.Any<Messaging.Request.ReceiveOnChannel>());
			objClient2.Received(1).Transmit(Arg.Any<Messaging.Request.ReceiveOnChannel>());

			//Act2
			objClient1.ClearReceivedCalls();
			objClient2.ClearReceivedCalls();

			obj.UnSub(objClient2, objUnSubMessage);
			obj.Notify(objClient1, objSendMessage);

			//Assert2
			objClient1.Received(0).Transmit(Arg.Any<Messaging.Request.ReceiveOnChannel>());
			objClient2.Received(0).Transmit(Arg.Any<Messaging.Request.ReceiveOnChannel>());
		}
	}
}
