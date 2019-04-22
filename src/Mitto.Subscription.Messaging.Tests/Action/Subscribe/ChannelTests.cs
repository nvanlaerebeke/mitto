using Mitto.IMessaging;
using Mitto.Messaging.Response;
using Mitto.Subscription.Messaging.Handlers;
using NSubstitute;
using NUnit.Framework;

namespace Mitto.Subscription.Messaging.Tests.Action.Subscribe {

    [TestFixture]
    public class ChannelTests {

        /// <summary>
        /// Tests the creation of the Channel subscription message
        /// This means that the subscription handler is gotten from the
        /// IMessageProvider and that the Sub() method on it is called
        /// The response is expected to be an ACK message
        /// </summary>
        [Test]
        public void CreateTest() {
            //Arrange
            var objProvider = Substitute.For<IMessageProvider>();
            var objClient = Substitute.For<IClient>();
            var objRequestMessage = Substitute.For<Messaging.Subscribe.ChannelSubscribe>("MyChannel");
            var objSubscriptionHandler = Substitute.For<ChannelSubscriptionHandler>();

            objRequestMessage.ID.Returns("MyRequestID");
            objProvider.GetSubscriptionHandler<ChannelSubscriptionHandler>().Returns(objSubscriptionHandler);
            objSubscriptionHandler.Sub(Arg.Is(objClient.Router), Arg.Is(objRequestMessage)).Returns(true);

            Config.Initialize(new Config.ConfigParams() {
                MessageProvider = objProvider
            });

            //Act
            var obj = new Messaging.Action.Subscribe.ChannelSubscribeAction(objClient, objRequestMessage);
            var objResponse = obj.Start() as ACKResponse;

            //Assert
            Assert.NotNull(obj);
            Assert.NotNull(objResponse);
            Assert.AreEqual("MyRequestID", objResponse.ID);
            Assert.AreEqual(ResponseState.Success, objResponse.Status.State);
            objProvider.Received(1).GetSubscriptionHandler<ChannelSubscriptionHandler>();
            objSubscriptionHandler.Received(1).Sub(Arg.Is(objClient.Router), Arg.Is(objRequestMessage));
        }
    }
}