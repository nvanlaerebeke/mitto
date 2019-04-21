using Mitto.IMessaging;
using Mitto.IRouting;
using Mitto.Messaging.Action.Request;
using Mitto.Messaging.Notification;
using Mitto.Messaging.Request;
using Mitto.Messaging.Response;
using Mitto.Messaging.Tests.TestData.Action.Request;
using Mitto.Messaging.Tests.TestData.Notification;
using Mitto.Messaging.Tests.TestData.Request;
using Mitto.Messaging.Tests.TestData.Response;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Mitto.Messaging.MessageProvider;

namespace Mitto.Messaging.Tests {

    [TestFixture]
    public class MessageProviderTests {

        /// <summary>
        /// Tests getting an action for the provided IMessage where the action does not exist
        /// This means the expected return value should be null
        /// </summary>
        [Test]
        public void GetActionForUnknownMessageTest() {
            //Arrange
            var objClient = Substitute.For<IClient>();
            var objMessage = Substitute.For<IRequestMessage>();

            objMessage.Type.Returns(MessageType.Request);
            objMessage.Name.Returns("NoneExistingMessage");

            //Act
            var objProvider = new MessageProvider();
            var objAction = objProvider.GetAction(objClient, objMessage);

            //Assert
            Assert.IsNull(objAction);
        }

        /// <summary>
        /// Tests getting an action for the provided IMessage
        /// This means that the expected return value should be an IAction
        /// </summary>
        [Test]
        public void GetActionTest() {
            //Arrange
            var objClient = Substitute.For<IClient>();
            var objMessage = Substitute.For<Request.EchoRequest>();
            objMessage.Name.Returns("EchoRequest");

            //Act
            var objProvider = new MessageProvider();
            var objAction = objProvider.GetAction(objClient, objMessage);

            //Assert
            Assert.NotNull(objAction);
        }

        /// <summary>
        /// Tests getting a message from the provider based on a byte array
        /// This means that the IMessageConverter expects the byte[] that represent
        /// the actual message without the MessageType and message name
        ///
        /// See Frame for more info about how the byte[] is build
        /// </summary>
        [Test]
        public void GetMessageTest() {
            //Arrange
            var objConverter = Substitute.For<IMessageConverter>();
            var arrID = Encoding.UTF32.GetBytes("MyID");
            var arrName = Encoding.UTF32.GetBytes("EchoRequest");
            var arrData = new byte[] { 1, 2, 3, 4, 5 };

            var lstBytes = new List<byte>();
            lstBytes.Add((byte)MessageType.Request);
            lstBytes.Add((byte)arrID.Length);
            lstBytes.AddRange(arrID);
            lstBytes.Add((byte)arrName.Length);
            lstBytes.AddRange(arrName);
            lstBytes.AddRange(arrData);

            var objMessage = Substitute.For<IMessage>();
            objConverter.GetMessage(Arg.Is(typeof(Request.EchoRequest)), Arg.Is<byte[]>(b => b.SequenceEqual(arrData))).Returns(objMessage);

            Config.Initialize(new Config.ConfigParams() {
                MessageConverter = objConverter
            });

            //Act
            var objReturnMessage = new MessageProvider().GetMessage(lstBytes.ToArray());

            //Assert
            objConverter.Received(1).GetMessage(typeof(Request.EchoRequest), Arg.Is<byte[]>(b => b.SequenceEqual(arrData)));
            Assert.NotNull(objMessage);
            Assert.AreEqual(objMessage, objReturnMessage);
        }

        /// <summary>
        /// Tests getting an IResponseMessage for the given IMessage with the provided code
        /// </summary>
        [Test]
        public void GetResponseMessageTest() {
            //Arrange
            var objMessage = new Request.EchoRequest("MyMessage"); // Substitute.For<Request.Echo>();

            //Act
            var objReturnMessage = new MessageProvider().GetResponseMessage(objMessage, new ResponseStatus(ResponseState.TimeOut));

            //Assert
            Assert.NotNull(objReturnMessage);
            Assert.AreEqual("EchoResponse", objReturnMessage.Name);
            Assert.AreEqual(MessageType.Response, objReturnMessage.Type);
            Assert.AreEqual(ResponseState.TimeOut, objReturnMessage.Status.State);
        }

        /// <summary>
        /// Tests the initialization of the MessgeProvider
        /// This means the types in Mitto.Messaging are expected in the
        /// Types and Actions properties present in the MessageProvider class
        /// </summary>
        [Test]
        public void TestLoadTypesDefault() {
            //Arrange & Act
            var objProvider = new MessageProvider();

            var dicRequests = new Dictionary<string, Type>() {
                { "RequestTestMessage", typeof(RequestTestMessage) },
                { "NotificationTestMessage", typeof(NotificationTestMessage) },
                { "InfoNotification", typeof(InfoNotification) },
                { "LogStatusNotification", typeof(LogStatusNotification) },
                { "EchoRequest", typeof(EchoRequest) },
                { "PingRequest", typeof(PingRequest) }
            };

            var dicResponses = new Dictionary<string, Type>() {
                { "ResponseTestMessage", typeof(ResponseTestMessage) },
                { "ACKResponse", typeof(ACKResponse) },
                { "EchoResponse", typeof(EchoResponse) },
                { "PongResponse", typeof(PongResponse) }
            };

            var lstActions = new List<ActionInfo>() {
                //{ "NotificationTestAction", new ActionInfo() },
                { new ActionInfo(typeof(RequestTestMessage), typeof(ResponseTestMessage), typeof(RequestTestAction)) },
                { new ActionInfo(typeof(EchoRequest), typeof(EchoResponse), typeof(EchoRequestAction)) },
                { new ActionInfo(typeof(PingRequest), typeof(PongResponse), typeof(PingRequestAction)) }
            };

            //Assert
            foreach (var kvp in dicRequests) {
                Assert.IsTrue(
                    objProvider.Requests.ContainsKey(kvp.Key) &&
                    objProvider.Requests[kvp.Key].FullName.Equals(kvp.Value.FullName)
                );
            }

            foreach (var kvp in dicResponses) {
                Assert.IsTrue(
                    objProvider.Responses.ContainsKey(kvp.Key) &&
                    objProvider.Responses[kvp.Key].FullName.Equals(kvp.Value.FullName)
                );
            }

            foreach (var obj in lstActions) {
                Assert.IsTrue(objProvider.Actions.Contains(obj));
            }
        }
    }
}