using Mitto.IMessaging;
using Mitto.Messaging.Response;
using Mitto.Subscription.IMessaging.Handlers;
using Mitto.Subscription.Messaging.Action.UnSubscribe;
using Mitto.Subscription.Messaging.UnSubscribe;
using NSubstitute;
using NUnit.Framework;

namespace Mitto.Subscription.Messaging.Tests.Action.UnSubscribe {

    [TestFixture]
    public class ChannelTests {

        /// <summary>
        /// Tests the creation of the Channel unsubscribe message
        /// This means that the subscription handler is gotten from the
        /// IMessageProvider and that the UnSub() method on it is called
        /// The response is expected to be an ACK message
        /// </summary>
        [Test]
        public void CreateTest() {
            //Arrange
            var objProvider = Substitute.For<IMessageProvider>();
            var objClient = Substitute.For<IClient>();
            var objRequestMessage = Substitute.For<ChannelUnSubscribe>("MyChannel");
            var objSubscriptionHandler = Substitute.For<IChannelSubscriptionHandler>();

            objRequestMessage.ID.Returns("MyRequestID");
            objProvider.GetSubscriptionHandler<IChannelSubscriptionHandler>().Returns(objSubscriptionHandler);
            objSubscriptionHandler.UnSub(Arg.Is(objClient.Router), Arg.Is(objRequestMessage)).Returns(true);

            Config.Initialize(new Config.ConfigParams() {
                MessageProvider = objProvider
            });

            //Act
            var obj = new ChannelUnSubscribeAction(objClient, objRequestMessage);
            var objResponse = obj.Start() as ACKResponse;

            //Assert
            Assert.NotNull(obj);
            Assert.NotNull(objResponse);
            Assert.AreEqual("MyRequestID", objResponse.ID);
            Assert.AreEqual(ResponseState.Success, objResponse.Status.State);
            objProvider.Received(1).GetSubscriptionHandler<IChannelSubscriptionHandler>();
            objSubscriptionHandler.Received(1).UnSub(Arg.Is(objClient.Router), Arg.Is(objRequestMessage));
        }
    }
}