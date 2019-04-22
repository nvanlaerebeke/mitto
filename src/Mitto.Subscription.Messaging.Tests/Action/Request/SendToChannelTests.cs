using Mitto.IMessaging;
using Mitto.Messaging.Response;
using Mitto.Subscription.Messaging.Handlers;
using NSubstitute;
using NUnit.Framework;

namespace Mitto.Subscription.Messaging.Tests.Action.Request {

    [TestFixture]
    public class SendToChannelTests {

        /// <summary>
        /// Tests the SendToChannel action for the requests message SendToChannel
        /// This means an Response.ACK message is expected as response and
        /// that the IMessageProvider.GetSubscriptionHandler is called to get the
        /// subscription handler where the Notify method will be called on
        /// </summary>
        [Test]
        public void CreateTest() {
            //Arrange
            var objProvider = Substitute.For<IMessageProvider>();
            var objClient = Substitute.For<IClient>();
            var objMessage = Substitute.For<Messaging.Request.SendToChannelRequest>("MyChannel", "MyMessage");
            var objSubscriptionHandler = Substitute.For<ChannelSubscriptionHandler>();

            objMessage.ID.Returns("MyRequestID");

            objProvider.GetSubscriptionHandler<ChannelSubscriptionHandler>().Returns(objSubscriptionHandler);
            objSubscriptionHandler.Notify(Arg.Is(objClient.Router), Arg.Is(objMessage)).Returns(true);

            Config.Initialize(new Config.ConfigParams() {
                MessageProvider = objProvider
            });

            //Act
            var obj = new SendToChannelRequestAction(objClient, objMessage);
            var objResponse = obj.Start() as ACKResponse;

            //Assert
            Assert.IsNotNull(obj);
            Assert.IsNotNull(objResponse);
            Assert.AreEqual("MyRequestID", objResponse.ID);
            Assert.AreEqual(ResponseState.Success, objResponse.Status.State);
            objProvider.Received(1).GetSubscriptionHandler<ChannelSubscriptionHandler>();
            objSubscriptionHandler.Received(1).Notify(Arg.Is(objClient.Router), Arg.Is(objMessage));
        }
    }
}