using Mitto.Connection.WebsocketSharp.Client;
using Mitto.IConnection;
using Mitto.Utilities;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using WebSocketSharp;

namespace Mitto.Connection.WebsocketSharp.Tests.Client {

    [TestFixture]
    public class WebsocketClientTests {

        [SetUp]
        public void SetUp() {
            Config.Initialize();
        }

        /// <summary>
        /// Tests the timeout event for the keep-alive monitor
        /// This means that when the TimeOut event is called from the IKeepAliveMonitor class
        /// the IKeepAliveMonitor.StartCountDown() is called
        /// </summary>
        [Test]
        public void KeepAliveTimeOutPingFailedTest() {
            //Arrange
            var objWebSocketClient = Substitute.For<IWebSocketClient>();
            var objKeepAliveMonitor = Substitute.For<IKeepAliveMonitor>();
            var objClient = new WebsocketClient(objWebSocketClient, objKeepAliveMonitor);
            objWebSocketClient.Ping().Returns(false);

            //Act
            objClient.ConnectAsync(new ClientParams() { Hostname = "localhost", Port = 80, Secure = false });
            objKeepAliveMonitor.TimeOut += Raise.EventWith(this, new EventArgs());

            //Assert
            objKeepAliveMonitor.Received(1).StartCountDown();
            objWebSocketClient.Received(1).Ping();
            objKeepAliveMonitor.Received(0).Reset();
        }

        /// <summary>
        /// Tests the timeout event for the keep-alive monitor
        /// This means that when the TimeOut event is called from the IKeepAliveMonitor class
        /// the IKeepAliveMonitor.StartCountDown() is called
        /// </summary>
        [Test]
        public void KeepAliveTimeOutPingOKTest() {
            //Arrange
            var objWebSocketClient = Substitute.For<IWebSocketClient>();
            var objKeepAliveMonitor = Substitute.For<IKeepAliveMonitor>();
            var objClient = new WebsocketClient(objWebSocketClient, objKeepAliveMonitor);
            objWebSocketClient.Ping().Returns(true);

            //Act
            objClient.ConnectAsync(new ClientParams() { Hostname = "localhost", Port = 80, Secure = false });
            objKeepAliveMonitor.TimeOut += Raise.EventWith(this, new EventArgs());

            //Assert
            objKeepAliveMonitor.Received(1).StartCountDown();
            objWebSocketClient.Received(1).Ping();
            objKeepAliveMonitor.Received(1).Reset();
        }

        /// <summary>
        /// Tests the UnResponsive event from the keep-alive monitor
        /// This means that the Disconnect method is called in the
        /// </summary>
        [Test]
        public void KeepAliveUnResponsiveTest() {
            //Arrange
            var objWebSocketClient = Substitute.For<IWebSocketClient>();
            var objKeepAliveMonitor = Substitute.For<IKeepAliveMonitor>();
            var objClient = Substitute.ForPartsOf<WebsocketClient>(objWebSocketClient, objKeepAliveMonitor);
            objClient.WhenForAnyArgs(x => x.Disconnect()).DoNotCallBase();

            //Act
            objKeepAliveMonitor.UnResponsive += Raise.EventWith(this, new EventArgs());

            //Assert
            objClient.Received(1).Disconnect();
        }

        /// <summary>
        /// Tests the close function
        /// This means verifying that all event subscriptions are gone
        /// and the disconnect  event was called
        /// </summary>
        [Test]
        public void DisconnectTest() {
            //Arrange
            var objWebSocketClient = Substitute.For<IWebSocketClient>();
            var objKeepAliveMonitor = Substitute.For<IKeepAliveMonitor>();
            var objClient = new WebsocketClient(objWebSocketClient, objKeepAliveMonitor);
            var objHandler = Substitute.For<EventHandler<IConnection.IConnection>>();
            objClient.Disconnected += objHandler;

            //Act
            objClient.ConnectAsync(new ClientParams() { Hostname = "localhost", Port = 80, Secure = false });
            objClient.Disconnect();

            //Assert
            objWebSocketClient.Received(1).OnOpen -= Arg.Any<EventHandler>();
            objWebSocketClient.Received(1).OnClose -= Arg.Any<EventHandler<ICloseEventArgs>>();
            objWebSocketClient.Received(1).OnError -= Arg.Any<EventHandler<IErrorEventArgs>>();
            objWebSocketClient.Received(1).OnMessage -= Arg.Any<EventHandler<IMessageEventArgs>>();
            objKeepAliveMonitor.Received(1).TimeOut -= Arg.Any<EventHandler>();
            objKeepAliveMonitor.Received(1).UnResponsive -= Arg.Any<EventHandler>();

            objKeepAliveMonitor.Received(1).Stop();

            objHandler
                .Received(1)
                .Invoke(Arg.Is(objClient), Arg.Is(objClient))
            ;
        }

        /// <summary>
        /// Tests the ConnectAsync method
        /// This means checking if the event handlers were added on IWebSocketClient
        /// </summary>
        [Test]
        public void ConnectAsyncTest() {
            //Arrange
            var objWebSocketClient = Substitute.For<IWebSocketClient>();
            var objKeepAliveMonitor = Substitute.For<IKeepAliveMonitor>();
            var objClient = new WebsocketClient(objWebSocketClient, objKeepAliveMonitor);

            //Act
            objClient.ConnectAsync(new ClientParams() {
                Hostname = "localhost",
                Port = 80,
                Secure = false,
                ConnectionTimeoutSeconds = 666
            });

            //Assert
            objWebSocketClient.Received(1).OnOpen += Arg.Any<EventHandler>();
            objWebSocketClient.Received(1).OnClose += Arg.Any<EventHandler<ICloseEventArgs>>();
            objWebSocketClient.Received(1).OnError += Arg.Any<EventHandler<IErrorEventArgs>>();
            objWebSocketClient.Received(1).OnMessage += Arg.Any<EventHandler<IMessageEventArgs>>();
            objKeepAliveMonitor.Received(1).TimeOut += Arg.Any<EventHandler>();
            objKeepAliveMonitor.Received(1).UnResponsive += Arg.Any<EventHandler>();

            Assert.AreEqual(666, objWebSocketClient.ConnectionTimeoutSeconds);
        }

        /// <summary>
        /// Tests the connected event
        /// This means that after OnOpen is triggered on IWebSocketClient a Connected event
        /// is expected for the WebsocketClient
        /// </summary>
        [Test]
        public void ConnectedTest() {
            //Arrange
            var objWebSocketClient = Substitute.For<IWebSocketClient>();
            var objKeepAliveMonitor = Substitute.For<IKeepAliveMonitor>();
            var objClient = new WebsocketClient(objWebSocketClient, objKeepAliveMonitor);
            var handler = Substitute.For<EventHandler<IConnection.IClient>>();
            objClient.Connected += handler;

            //Act
            objClient.ConnectAsync(new ClientParams() { Hostname = "localhost", Port = 80, Secure = false });
            objWebSocketClient.Received().OnOpen += Raise.EventWith(objWebSocketClient, new EventArgs());

            //Assert
            objKeepAliveMonitor.Received(1).Start();
            handler
                .Received(1)
                .Invoke(Arg.Is(objClient), Arg.Is(objClient))
            ;
        }

        /// <summary>
        /// Test the constructing of a WebsocketClient
        /// This means that the ID is set to a GUID
        /// </summary>
        [Test]
        public void CreateTest() {
            //Arrange
            var objWebSocketClient = Substitute.For<WebSocketClientWrapper>();
            var objKeepAliveMonitor = Substitute.For<IKeepAliveMonitor>();

            //Act
            var objClient = new WebsocketClient(objWebSocketClient, objKeepAliveMonitor);

            //Assert
            Guid result = new Guid();
            Assert.IsTrue(Guid.TryParse(objClient.ID, out result));
        }

        /// <summary>
        /// Tests if the disconnect event is fired on OnClose
        /// This means testing if the event handlers are gone and that
        /// Close was called when the connection state != Closed or Closing
        /// </summary>
        [Test, Sequential]
        public void DisconnectedOnCloseTest(
            [Values(WebSocketState.Closed, WebSocketState.Closing, WebSocketState.Connecting, WebSocketState.Open)] WebSocketState state,
            [Values(false, false, true, true)] bool expectclose
        ) {
            //Arrange
            var objWebSocketClient = Substitute.For<IWebSocketClient>();
            var objKeepAliveMonitor = Substitute.For<IKeepAliveMonitor>();
            var objClient = new WebsocketClient(objWebSocketClient, objKeepAliveMonitor);
            var eventArgs = Substitute.For<ICloseEventArgs>();
            objWebSocketClient.ReadyState.Returns(state);

            var handler = Substitute.For<EventHandler<IConnection.IConnection>>();
            objClient.Disconnected += handler;

            //Act
            objClient.ConnectAsync(new ClientParams() { Hostname = "localhost", Port = 80, Secure = false });
            objWebSocketClient.OnClose += Raise.Event<EventHandler<ICloseEventArgs>>(objClient, eventArgs);

            //Assert
            objWebSocketClient.Received().OnOpen -= Arg.Any<EventHandler>();
            objWebSocketClient.Received().OnClose -= Arg.Any<EventHandler<ICloseEventArgs>>();
            objWebSocketClient.Received().OnError -= Arg.Any<EventHandler<IErrorEventArgs>>();
            objWebSocketClient.Received().OnMessage -= Arg.Any<EventHandler<IMessageEventArgs>>();

            objWebSocketClient.Received((expectclose) ? 1 : 0).Close();

            handler
                .Received(1)
                .Invoke(Arg.Is(objClient), Arg.Is(objClient))
            ;
        }

        /// <summary>
        /// Tests if the disconnect event is fired on OnClose
        /// This means testing if the event handlers are gone and that
        /// Close was called when the connection state != Closed or Closing
        /// </summary>
        [Test, Sequential]
        public void DisconnectedOnErrorTest(
            [Values(WebSocketState.Closed, WebSocketState.Closing, WebSocketState.Connecting, WebSocketState.Open)] WebSocketState state,
            [Values(false, false, true, true)] bool expectclose
        ) {
            //Arrange
            var objWebSocketClient = Substitute.For<IWebSocketClient>();
            var objKeepAliveMonitor = Substitute.For<IKeepAliveMonitor>();
            var objClient = new WebsocketClient(objWebSocketClient, objKeepAliveMonitor);
            var eventArgs = Substitute.For<IErrorEventArgs>();
            var handler = Substitute.For<EventHandler<IConnection.IConnection>>();
            objWebSocketClient.ReadyState.Returns(state);
            objClient.Disconnected += handler;

            //Act
            objClient.ConnectAsync(new ClientParams() { Hostname = "localhost", Port = 80, Secure = false });
            objWebSocketClient.OnError += Raise.Event<EventHandler<IErrorEventArgs>>(objClient, eventArgs);

            //Assert
            objWebSocketClient.Received().OnOpen -= Arg.Any<EventHandler>();
            objWebSocketClient.Received().OnClose -= Arg.Any<EventHandler<ICloseEventArgs>>();
            objWebSocketClient.Received().OnError -= Arg.Any<EventHandler<IErrorEventArgs>>();
            objWebSocketClient.Received().OnMessage -= Arg.Any<EventHandler<IMessageEventArgs>>();

            objWebSocketClient.Received((expectclose) ? 1 : 0).Close();

            handler
                .Received(1)
                .Invoke(
                    Arg.Is<IConnection.IConnection>(c => c.Equals(objClient)),
                    Arg.Is(objClient)
                )
            ;
        }

        /// <summary>
        /// Tests the message received event (Rx) with binary data
        /// This means that when OnMessage is triggered an Rx event with the binary data is expected
        /// </summary>
        [Test]
        public void RxBInaryTest() {
            //Arrange
            var objWebSocketClient = Substitute.For<IWebSocketClient>();
            var objKeepAliveMonitor = Substitute.For<IKeepAliveMonitor>();
            var objClient = new WebsocketClient(objWebSocketClient, objKeepAliveMonitor);
            var handler = Substitute.For<EventHandler<byte[]>>();
            var eventArgs = Substitute.For<IMessageEventArgs>();
            eventArgs.IsText.Returns(false);
            eventArgs.IsPing.Returns(false);
            eventArgs.IsBinary.Returns(true);

            objClient.Rx += handler;
            eventArgs.RawData.Returns(new byte[] { 1, 2, 3, 4 });

            //Act
            objClient.ConnectAsync(new ClientParams() { Hostname = "localhost", Port = 80, Secure = false });
            objWebSocketClient.OnMessage += Raise.Event<EventHandler<IMessageEventArgs>>(objClient, eventArgs);

            //Assert
            objKeepAliveMonitor.Received(1).Reset();
            handler
                .Received(1)
                .Invoke(Arg.Is<IConnection.IConnection>(m => m.Equals(objClient)), Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })))
            ;
        }

        /// <summary>
        /// Tests the message received event (Rx) with text data
        /// This means that when OnMessage is triggered an Rx event with the binary data is expected
        /// </summary>
        [Test]
        public void RxTextTest() {
            //Arrange
            var objWebSocketClient = Substitute.For<IWebSocketClient>();
            var objKeepAliveMonitor = Substitute.For<IKeepAliveMonitor>();
            var objClient = new WebsocketClient(objWebSocketClient, objKeepAliveMonitor);
            var handler = Substitute.For<EventHandler<byte[]>>();
            var eventArgs = Substitute.For<IMessageEventArgs>();
            eventArgs.IsText.Returns(true);
            eventArgs.IsPing.Returns(false);
            eventArgs.IsBinary.Returns(false);

            objClient.Rx += handler;
            eventArgs.Data.Returns("TEST");

            //Act
            objClient.ConnectAsync(new ClientParams() { Hostname = "localhost", Port = 80, Secure = false });
            objWebSocketClient.OnMessage += Raise.Event<EventHandler<IMessageEventArgs>>(objClient, eventArgs);

            //Assert
            objKeepAliveMonitor.Received(1).Reset();
            handler
                .Received(1)
                .Invoke(Arg.Is<IConnection.IConnection>(m => m.Equals(objClient)), Arg.Is<byte[]>(b => b.SequenceEqual(System.Text.Encoding.UTF32.GetBytes("TEST"))))
            ;
        }

        /// <summary>
        /// Tests the message received event (Rx) with ping data
        /// This means that when OnMessage is triggered no Rx event is expected
        /// </summary>
        [Test]
        public void RxPingTest() {
            //Arrange
            var objWebSocketClient = Substitute.For<IWebSocketClient>();
            var objKeepAliveMonitor = Substitute.For<IKeepAliveMonitor>();
            var objClient = new WebsocketClient(objWebSocketClient, objKeepAliveMonitor);
            var handler = Substitute.For<EventHandler<byte[]>>();
            var eventArgs = Substitute.For<IMessageEventArgs>();
            eventArgs.IsText.Returns(false);
            eventArgs.IsPing.Returns(true);
            eventArgs.IsBinary.Returns(false);

            objClient.Rx += handler;

            //Act
            objClient.ConnectAsync(new ClientParams() { Hostname = "localhost", Port = 80, Secure = false });
            objWebSocketClient.OnMessage += Raise.Event<EventHandler<IMessageEventArgs>>(objClient, eventArgs);

            //Assert
            objKeepAliveMonitor.Received(1).Reset();
            handler
                .Received(0)
                .Invoke(Arg.Any<IConnection.IConnection>(), Arg.Any<byte[]>())
            ;
        }

        /// <summary>
        /// Tests the Transmit method
        /// This means that when binary data is passed to the Transmit method a call on the IWebSockClient.Send
        /// is expected in the very near future (sender queue = different thread)
        /// </summary>
        [Test]
        public void TransmitTest() {
            //Arrange
            var objWebSocketClient = Substitute.For<IWebSocketClient>();
            var objKeepAliveMonitor = Substitute.For<IKeepAliveMonitor>();
            var objClient = new WebsocketClient(objWebSocketClient, objKeepAliveMonitor);
            objClient.ConnectAsync(new ClientParams() { Hostname = "localhost", Port = 80, Secure = false });
            objWebSocketClient.Received().OnOpen += Raise.EventWith(objWebSocketClient, new EventArgs());

            //Act
            objClient.Transmit(new byte[] { 1, 2, 3, 4 });
            Thread.Sleep(50); //-- wait a bit so the sender queue can call the IWebSocketClient.Send() method

            //Assert
            objWebSocketClient.Received(1).SendAsync(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })));
        }
    }
}