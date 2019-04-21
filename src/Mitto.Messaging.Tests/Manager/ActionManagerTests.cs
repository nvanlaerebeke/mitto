using NUnit.Framework;
using NSubstitute;
using System;
using System.Linq;
using Mitto.IMessaging;
using System.Threading.Tasks;
using System.Threading;
using Mitto.IRouting;

namespace Mitto.Messaging.Tests {

    [TestFixture]
    public class ActionManagerTests {

        /// <summary>
        /// Test if when passing null as IAction
        /// This test if no exceptions are raised
        /// </summary>
        [Test]
        public void NullActionTest() {
            //Arrange
            var objClient = Substitute.For<IClient>();
            var objMessage = Substitute.For<IRequestMessage>();
            //Act
            new ActionManager().RunAction(objClient, objMessage, null);
        }

        /// <summary>
        /// Tests if an unknown exception is handled correctly
        /// This means that process is called with an IMessage and raises an exception
        /// Based on the message type a response message(Transmit) is expected
        /// </summary>
        /// <param name="pType"></param>
        /// <param name="pTransmitExpected"></param>
        [Test, Sequential]
        public void ProcessUnknownExceptionTest(
            [Values(
                MessageType.Notification,
                MessageType.Request
            )] MessageType pType,
            [Values(
                false,
                true
            )] bool pTransmitExpected
        ) {
            //Arrange
            var objProvider = Substitute.For<IMessageProvider>();

            var objClient = Substitute.For<IClient>();
            var objMessage = Substitute.For<IRequestMessage>();
            var objAction = Substitute.For<IAction>();
            var objResponse = Substitute.For<IResponseMessage>();

            objMessage.Type.Returns(pType);

            if (pTransmitExpected) {
                objAction = Substitute.For<IRequestAction<IResponseMessage>>();
                objAction.When(a => ((IRequestAction<IResponseMessage>)a).Start()).Do(a => throw new Exception("Some Exception"));
            } else {
                objAction = Substitute.For<INotificationAction>();
                objAction.When(a => ((INotificationAction)a).Start()).Do(a => throw new Exception("Some Exception"));
            }
            objProvider.GetResponseMessage(Arg.Is(objMessage), Arg.Is<ResponseStatus>(r => r.State == ResponseState.Error)).Returns(objResponse);

            Config.Initialize(new Config.ConfigParams() {
                MessageProvider = objProvider
            });

            //Act
            new ActionManager().RunAction(objClient, objMessage, objAction);

            System.Threading.Thread.Sleep(50);

            //Assert
            if (pTransmitExpected) {
                ((IRequestAction<IResponseMessage>)objAction).Received(1).Start();
                objProvider.Received(1).GetResponseMessage(Arg.Any<IRequestMessage>(), Arg.Is<ResponseStatus>(r => r.State == ResponseState.Error));
                objClient.Received(1).Transmit(Arg.Is(objResponse));
            } else {
                ((INotificationAction)objAction).Received(1).Start();
                objClient.Received(0).Transmit(Arg.Any<IMessage>());
            }
        }

        /// <summary>
        /// Testing what happens when an action raises a MessageException
        /// This means that the Action is created by the IMessageProvider and
        /// the action quits with a MessageException where the status code is used
        /// to return the response message
        /// </summary>
        [Test]
        public void ProcessMessageExceptionTest() {
            //Arrange
            var objProvider = Substitute.For<IMessageProvider>();

            var objClient = Substitute.For<IClient>();
            var objMessage = Substitute.For<IRequestMessage>();
            var objAction = Substitute.For<IRequestAction<IResponseMessage>>();

            var objResponse = Substitute.For<IResponseMessage>();

            Config.Initialize(new Config.ConfigParams() {
                MessageProvider = objProvider
            });

            objMessage.Type.Returns(MessageType.Request);
            objAction.When(a => a.Start()).Do(a => throw new MessagingException(new ResponseStatus(ResponseState.Cancelled)));

            objProvider.GetResponseMessage(Arg.Any<IRequestMessage>(), Arg.Any<ResponseStatus>()).Returns(objResponse);

            //Act
            new ActionManager().RunAction(objClient, objMessage, objAction);

            System.Threading.Thread.Sleep(50);

            //Assert
            objAction.Received(1).Start();
            objProvider.Received(1).GetResponseMessage(Arg.Is(objMessage), Arg.Is<ResponseStatus>(r => r.State == ResponseState.Cancelled));
            objClient.Received(1).Transmit(Arg.Is(objResponse));
        }

        /// <summary>
        /// Tests handling a Notification Action
        /// This means that the NotificationAction.Start() is called and no Transmit is never called
        /// </summary>
        [Test]
        public void ProcessNotificationMessageTest() {
            //Arrange
            var objClient = Substitute.For<IClient>();
            var objMessage = Substitute.For<IRequestMessage>();
            var objAction = Substitute.For<INotificationAction>();

            var objProvider = Substitute.For<IMessageProvider>();

            objMessage.Type.Returns(MessageType.Notification);
            objProvider.GetMessage(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 }))).Returns(objMessage);
            objProvider.GetAction(objClient, objMessage).Returns(objAction);

            Config.Initialize(new Config.ConfigParams() {
                MessageProvider = objProvider
            });

            //Act
            new ActionManager().RunAction(objClient, objMessage, objAction);

            System.Threading.Thread.Sleep(50);

            //Assert
            objAction.Received(1).Start();
            objClient.Received(0).Transmit(Arg.Any<IMessage>());
        }

        /// <summary>
        /// Tests if a message is processed
        /// This means that a call is expected on the IConverter.GetMessage,
        /// that the Action is requested from the IConverter.GetAction and the IQueue.IQueue
        /// transmit method is called with the IResponseMessage byte[] from the action result
        /// </summary>
        [Test]
        public void ProcessRequestMessageTest() {
            //Arrange
            var objClient = Substitute.For<IClient>();
            var objMessage = Substitute.For<IRequestMessage>();
            var objAction = Substitute.For<IRequestAction<IResponseMessage>>();

            var objProvider = Substitute.For<IMessageProvider>();
            var objResponse = Substitute.For<IResponseMessage>();

            objMessage.Type.Returns(MessageType.Request);
            objAction.Start().Returns(objResponse);

            Config.Initialize(new Config.ConfigParams() {
                MessageProvider = objProvider
            });

            //Act
            new ActionManager().RunAction(objClient, objMessage, objAction);

            System.Threading.Thread.Sleep(50);

            //Assert
            objAction.Received(1).Start();
            objClient.Received(1).Transmit(Arg.Is(objResponse));
        }

        /// <summary>
        /// Tests the GetStatus method where a Busy state is expected
        /// This means that a RunAction is done, and then the GetStatus is called
        /// It's expected that the Status would be busy
        /// </summary>
        [Test]
        public void GetStatusBusyTest() {
            //Arrange
            var objClient = Substitute.For<IClient>();
            var objMessage = Substitute.For<IRequestMessage>();
            var objAction = Substitute.For<IRequestAction<IResponseMessage>>();

            var objProvider = Substitute.For<IMessageProvider>();
            var objResponse = Substitute.For<IResponseMessage>();

            objMessage.ID.Returns("MyID");
            objMessage.Type.Returns(MessageType.Request);

            objAction.Start().Returns(objResponse);
            objAction.When(a => a.Start()).Do(a => Thread.Sleep(50));

            Config.Initialize(new Config.ConfigParams() {
                MessageProvider = objProvider
            });

            //Act
            var obj = new ActionManager();
            Task.Run(() => {
                //Run the action on a different thread so the status can be gotten while it's busy
                obj.RunAction(objClient, objMessage, objAction);
            });
            Thread.Sleep(25);
            //Assert
            Assert.AreEqual(MessageStatus.Busy, obj.GetStatus(objMessage.ID));
        }

        /// <summary>
        /// Tests the GetStatus method where an UnKnwon state is expected
        /// This means that a GetStatus is done for a IMessage.ID that
        /// was never started
        /// </summary>
        [Test]
        public void GetStatusUnKnownTest() {
            //Arrange
            var objClient = Substitute.For<IClient>();
            var objMessage = Substitute.For<IMessage>();
            var objAction = Substitute.For<IRequestAction<IResponseMessage>>();

            //Act
            var obj = new ActionManager();

            //Assert
            Assert.AreEqual(MessageStatus.UnKnown, obj.GetStatus("MyID"));
        }
    }
}