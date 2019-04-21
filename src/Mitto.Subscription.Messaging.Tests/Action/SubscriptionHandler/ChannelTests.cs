using Mitto.IMessaging;
using Mitto.Subscription.Messaging.Tests.TestData.Action.SubscriptionHandler;
using Mitto.Subscription.Messaging.Handlers;
using NSubstitute;
using NUnit.Framework;
using Mitto.Messaging.Response;
using Mitto.Subscription.Messaging.Request;
using System;
using Mitto.Subscription.Messaging.Subscribe;
using Mitto.Subscription.Messaging.UnSubscribe;
using Mitto.IRouting;
using Mitto.Subscription.IMessaging.Request;

namespace Mitto.Subscription.Messaging.Tests.Action.SubscriptionHandler {

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
            var objMessageProcessor = Substitute.For<IMessageProcessor>();
            var objClient1 = Substitute.For<IClient>();
            var objClient2 = Substitute.For<IClient>();
            var objSendMessage = Substitute.For<SendToChannelRequest>("MyChannel", "MyMessage");
            var objSubMessage = Substitute.For<ChannelSubscribe>("MyChannel");

            objClient1.Equals(objClient1).Returns(true);

            Config.Initialize(new Config.ConfigParams() {
                MessageProcessor = objMessageProcessor
            });

            //Act
            var obj = new ChannelSubscriptionHandler();
            obj.Sub(objClient1.Router, objSubMessage);
            obj.Sub(objClient2.Router, objSubMessage);
            obj.Notify(objClient1.Router, objSendMessage);

            //Assert
            objMessageProcessor.Received(1).Request(Arg.Is(objClient2.Router), Arg.Is(objSendMessage), Arg.Any<Action<ACKResponse>>());
        }

        /// <summary>
        /// ToDo: find a way to test unsubscribe, ISubstitute objects are not removed
        /// </summary>
        [Test]
        public void UnSub() {
            //Arrange
            var objMessageProcessor = Substitute.For<IMessageProcessor>();
            var objClient1 = Substitute.For<IClient>();
            var objRouter1 = Substitute.For<IRouter>();
            var objClient2 = Substitute.For<IClient>();
            var objRouter2 = Substitute.For<IRouter>();
            var objSendMessage = Substitute.For<SendToChannelRequest>("MyChannel", "MyMessage");
            var objSubMessage = Substitute.For<ChannelSubscribe>("MyChannel");
            var objUnSubMessage = Substitute.For<ChannelUnSubscribe>("MyChannel");

            Config.Initialize(new Config.ConfigParams() {
                MessageProcessor = objMessageProcessor
            });

            objClient1.ID.Returns("Client1");
            objClient1.Router.Returns(objRouter1);
            objRouter1.ConnectionID.Returns("Router1");
            objClient1.Equals(Arg.Is(objClient1)).Returns(true);
            objClient2.ID.Returns("Client2");
            objClient1.Router.Returns(objRouter2);
            objRouter2.ConnectionID.Returns("Router2");

            //Act
            var obj = new ChannelSubscriptionHandler();
            obj.Sub(objClient1.Router, objSubMessage);
            obj.Sub(objClient2.Router, objSubMessage);
            obj.Notify(objClient1.Router, objSendMessage);

            //Assert
            objMessageProcessor.Received(1).Request(Arg.Is(objClient2.Router), Arg.Is(objSendMessage), Arg.Any<Action<ACKResponse>>());

            //Act2
            objMessageProcessor.ClearReceivedCalls();
            obj.UnSub(objClient2.Router, objUnSubMessage);
            obj.Notify(objClient1.Router, objSendMessage);

            //Assert2
            objMessageProcessor.Received(0).Request(Arg.Any<IRouter>(), Arg.Any<ISendToChannelRequest>(), Arg.Any<Action<ACKResponse>>());
        }
    }
}