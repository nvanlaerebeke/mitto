using Mitto.Connection.Websocket.Server;
using Mitto.IConnection;
using Mitto.Utilities;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;

namespace Mitto.Connection.Websocket.Tests.Server {
	[TestFixture]
	public class ClientTests {
		[SetUp]
		public void SetUp() {
			Config.Initialize();
		}

		/// <summary>
		/// Tests the timeout event for the kepalive monitor
		/// This means that when the TimeOut event is called from the IKeepAliveMonitor class
		/// the IKeepAliveMonitor.StartCountDown() is called
		/// </summary>
		[Test]
		public void KeepAliveTimeOutPingFailedTest() {
			//Arrange
			var objWebSocketBehavior = Substitute.For<IWebSocketBehavior>();
			var objKeepAliveMonitor = Substitute.For<IKeepAliveMonitor>();
			var objClient = new Websocket.Server.Client(objWebSocketBehavior, objKeepAliveMonitor);
			objWebSocketBehavior.Ping().Returns(false);

			//Act
			objKeepAliveMonitor.TimeOut += Raise.EventWith(this, new EventArgs());

			//Assert
			objKeepAliveMonitor.Received(1).StartCountDown();
			objWebSocketBehavior.Received(1).Ping();
			objKeepAliveMonitor.Received(0).Reset();
		}

		/// <summary>
		/// Tests the timeout event for the kepalive monitor
		/// This means that when the TimeOut event is called from the IKeepAliveMonitor class
		/// the IKeepAliveMonitor.StartCountDown() is called
		/// </summary>
		[Test]
		public void KeepAliveTimeOutPingOKTest() {
			//Arrange
			var objWebSocketBehavior = Substitute.For<IWebSocketBehavior>();
			var objKeepAliveMonitor = Substitute.For<IKeepAliveMonitor>();
			var objClient = new Websocket.Server.Client(objWebSocketBehavior, objKeepAliveMonitor);
			objWebSocketBehavior.Ping().Returns(true);

			//Act
			objKeepAliveMonitor.TimeOut += Raise.EventWith(this, new EventArgs());

			//Assert
			objKeepAliveMonitor.Received(1).StartCountDown();
			objWebSocketBehavior.Received(1).Ping();
			objKeepAliveMonitor.Received(1).Reset();
		}

		/// <summary>
		/// Tests the UnResponsive event from the keepalive monitor
		/// This means that the Disconnect method is called in the 
		/// </summary>
		[Test]
		public void KeepAliveUnResponsiveTest() {
			//Arrange
			var objWebSocketBehavior = Substitute.For<IWebSocketBehavior>();
			var objKeepAliveMonitor = Substitute.For<IKeepAliveMonitor>();
			var objClient = Substitute.ForPartsOf<Websocket.Server.Client>(objWebSocketBehavior, objKeepAliveMonitor);
			objClient.WhenForAnyArgs(x => x.Disconnect()).DoNotCallBase();

			//Act
			objKeepAliveMonitor.UnResponsive += Raise.EventWith(this, new EventArgs());

			//Assert
			objClient.Received(1).Disconnect();
		}

		/// <summary>
		/// Test closing a connection 
		/// This means the disconnect event is raised and there are no more subscriptions
		/// to the events from the IWebSocketBehavior
		/// 
		/// ToDo: Find a way to test the CancelationToken and SenderQueue thread
		/// </summary>
		[Test]
		public void DisconnectTest() {
			//Setup
			var objWebSocketBehavior = Substitute.For<IWebSocketBehavior>();
			var objKeepAliveMonitor = Substitute.For<IKeepAliveMonitor>();
			var objClient = new Websocket.Server.Client(objWebSocketBehavior, objKeepAliveMonitor);
			var objHandler = Substitute.For<EventHandler>();
			objClient.Disconnected += objHandler;

			//Act
			objClient.Disconnect();

			//Assert
			objKeepAliveMonitor.Received(1).TimeOut -= Arg.Any<EventHandler>();
			objKeepAliveMonitor.Received(1).UnResponsive -= Arg.Any<EventHandler>();
			objWebSocketBehavior.Received(1).OnCloseReceived -= Arg.Any<EventHandler<ICloseEventArgs>>();
			objWebSocketBehavior.Received(1).OnErrorReceived -= Arg.Any<EventHandler<IErrorEventArgs>>();
			objWebSocketBehavior.Received(1).OnMessageReceived -= Arg.Any<EventHandler<IMessageEventArgs>>();
			objKeepAliveMonitor.Received(1).Stop();
			objWebSocketBehavior.Received(1).Close();
				
			objHandler
				.Received(1)
				.Invoke(
					Arg.Is<IConnection.IConnection>(c => c.Equals(objClient)),
					Arg.Any<EventArgs>()
				)
			;
		}

		/// <summary>
		/// Test the creation of the Client class
		/// This means that the OnClose/OnError and OnMesage event handers are added and the ID is available 
		/// 
		/// ToDo: Find a way to test the senderqueue thread
		/// </summary>
		[Test]
		public void CreateTest() {
			//Setup
			var objWebSocketBehavior = Substitute.For<IWebSocketBehavior>();
			var objKeepAliveMonitor = Substitute.For<IKeepAliveMonitor>();

			//Act
			var objClient = new Websocket.Server.Client(objWebSocketBehavior, objKeepAliveMonitor);

			//Assert
			objKeepAliveMonitor.Received(1).TimeOut += Arg.Any<EventHandler>();
			objKeepAliveMonitor.Received(1).UnResponsive += Arg.Any<EventHandler>();
			objWebSocketBehavior.Received(1).OnCloseReceived += Arg.Any<EventHandler<ICloseEventArgs>>();
			objWebSocketBehavior.Received(1).OnErrorReceived += Arg.Any<EventHandler<IErrorEventArgs>>();
			objWebSocketBehavior.Received(1).OnMessageReceived += Arg.Any<EventHandler<IMessageEventArgs>>();

			objKeepAliveMonitor.Received(1).Start();
			Assert.IsTrue(Guid.TryParse(objClient.ID, out Guid objGuid));
		}

		/// <summary>
		/// Tests the Disconnect event when the WebSocketBehavior.OnClose is raised
		/// This means the Close method is being called
		/// </summary>
		[Test]
		public void DisconnectedOnCloseTest() {
			//Setup
			var objWebSocketBehavior = Substitute.For<IWebSocketBehavior>();
			var objKeepAliveMonitor = Substitute.For<IKeepAliveMonitor>();
			var objClient = new Websocket.Server.Client(objWebSocketBehavior, objKeepAliveMonitor);
			var objEventArgs = Substitute.For<ICloseEventArgs>();
			var objHandler = Substitute.For<EventHandler>();
			objClient.Disconnected += objHandler;

			//Act
			objWebSocketBehavior.OnCloseReceived += Raise.Event<EventHandler<ICloseEventArgs>>(objClient, objEventArgs);

			//Assert
			objWebSocketBehavior.Received().OnCloseReceived -= Arg.Any<EventHandler<ICloseEventArgs>>();
			objWebSocketBehavior.Received().OnErrorReceived -= Arg.Any<EventHandler<IErrorEventArgs>>();
			objWebSocketBehavior.Received().OnMessageReceived -= Arg.Any<EventHandler<IMessageEventArgs>>();

			objHandler
				.Received(1)
				.Invoke(
					Arg.Is<IConnection.IConnection>(c => c.Equals(objClient)),
					Arg.Any<EventArgs>()
				)
			;

		}

		/// <summary>
		/// Tests the Disconnect event when the WebSocketBehavior.OnError is raised
		/// This means the Close method is being called
		/// </summary>
		[Test]
		public void DisconnectedOnErrorTest() {
			//Setup
			var objWebSocketBehavior = Substitute.For<IWebSocketBehavior>();
			var objKeepAliveMonitor = Substitute.For<IKeepAliveMonitor>();
			var objClient = new Websocket.Server.Client(objWebSocketBehavior, objKeepAliveMonitor);
			var objEventArgs = Substitute.For<IErrorEventArgs>();
			var objHandler = Substitute.For<EventHandler>();
			objClient.Disconnected += objHandler;

			//Act
			objWebSocketBehavior.OnErrorReceived += Raise.Event<EventHandler<IErrorEventArgs>>(objClient, objEventArgs);

			//Assert
			objWebSocketBehavior.Received().OnCloseReceived -= Arg.Any<EventHandler<ICloseEventArgs>>();
			objWebSocketBehavior.Received().OnErrorReceived -= Arg.Any<EventHandler<IErrorEventArgs>>();
			objWebSocketBehavior.Received().OnMessageReceived -= Arg.Any<EventHandler<IMessageEventArgs>>();

			objHandler
				.Received(1)
				.Invoke(Arg.Is(objClient), Arg.Any<EventArgs>());
		}

		/// <summary>
		/// Test the Rx (receive) event for binary data
		/// This means that when WebSocketBehavior.OnMessage is triggered 
		/// an Rx event with the binary data is expected
		/// </summary>
		[Test]
		public void RxBinaryTest() {
			//Setup
			var objWebSocketBehavior = Substitute.For<IWebSocketBehavior>();
			var objKeepAliveMonitor = Substitute.For<IKeepAliveMonitor>();
			var objClient = new Websocket.Server.Client(objWebSocketBehavior, objKeepAliveMonitor);
			var objHandler = Substitute.For<EventHandler<byte[]>>();
			var objEventArgs = Substitute.For<IMessageEventArgs>();
			objEventArgs.IsBinary.Returns(true);
			objEventArgs.IsText.Returns(false);
			objEventArgs.IsPing.Returns(false);
			objEventArgs.RawData.Returns(new byte[] { 1, 2, 3, 4 });

			objClient.Rx += objHandler;

			//Act
			objWebSocketBehavior.OnMessageReceived += Raise.Event<EventHandler<IMessageEventArgs>>(objClient, objEventArgs);

			//Assert
			objHandler
				.Received(1)
				.Invoke(Arg.Is<IConnection.IConnection>(c => c.Equals(objClient)), Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })))
			;
		}

		/// <summary>
		/// Test the Rx (receive) event when receiving a ping
		/// This means that when WebSocketBehavior.OnMessage is triggered for a ping
		/// no Rx event is expected, this is ignored
		/// </summary>
		[Test]
		public void RxPingTest() {
			//Setup
			var objWebSocketBehavior = Substitute.For<IWebSocketBehavior>();
			var objKeepAliveMonitor = Substitute.For<IKeepAliveMonitor>();
			var objClient = new Websocket.Server.Client(objWebSocketBehavior, objKeepAliveMonitor);
			var objEventArgs = Substitute.For<IMessageEventArgs>();
			objEventArgs.IsBinary.Returns(false);
			objEventArgs.IsText.Returns(false);
			objEventArgs.IsPing.Returns(true);

			var eventReceived = false;
			objClient.Rx += delegate(object sender, byte[] data) {
				eventReceived = true;
			};

			//Act
			objWebSocketBehavior.OnMessageReceived += Raise.Event<EventHandler<IMessageEventArgs>>(objClient, objEventArgs);

			//Assert
			Assert.IsFalse(eventReceived, "Event received for ping while this should have been ignored");
		}

		/// <summary>
		/// Test the Rx (receive) event for text data
		/// This means that when WebSocketBehavior.OnMessage is triggered 
		/// an Rx event with the binary data that represents the text
		/// </summary>
		[Test]
		public void RxTextTest() {
			//Setup
			var objWebSocketBehavior = Substitute.For<IWebSocketBehavior>();
			var objKeepAliveMonitor = Substitute.For<IKeepAliveMonitor>();
			var objClient = new Websocket.Server.Client(objWebSocketBehavior, objKeepAliveMonitor);
			var objHandler = Substitute.For<EventHandler<byte[]>>();
			var objEventArgs = Substitute.For<IMessageEventArgs>();
			objEventArgs.IsBinary.Returns(false);
			objEventArgs.IsText.Returns(true);
			objEventArgs.IsPing.Returns(false);
			objEventArgs.Data.Returns("MyString");

			objClient.Rx += objHandler;

			//Act
			objWebSocketBehavior.OnMessageReceived += Raise.Event<EventHandler<IMessageEventArgs>>(objClient, objEventArgs);

			//Assert
			var data = System.Text.Encoding.UTF32.GetBytes("MyString");
			objHandler
				.Received(1)
				.Invoke(
					Arg.Is<IConnection.IConnection>(c => c.Equals(objClient)), 
					Arg.Is<byte[]>(b => b.SequenceEqual(data))
				)
			;
		}

		/// <summary>
		/// Testing the transmit method
		/// This means that when transmitting binary data the WebSocketBehavior.Send() method is called
		/// with said binary data shortly after (runs on separate thread)
		/// </summary>
		[Test]
		public void TransmitTest() {
			//Setup
			var objWebSocketBehavior = Substitute.For<IWebSocketBehavior>();
			var objKeepAliveMonitor = Substitute.For<IKeepAliveMonitor>();
			var objClient = new Websocket.Server.Client(objWebSocketBehavior, objKeepAliveMonitor);

			//Act
			objClient.Transmit(new byte[] { 1, 2, 3, 4 });
			System.Threading.Thread.Sleep(50); // wait a bit, data is being transmitted to IWebSocketBehavior on a separate thread

			//Assert
			objWebSocketBehavior.Received(1).Send(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })));
		}
	}
}