using Mitto.Connection.Websocket.Server;
using Mitto.IConnection;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;

namespace Mitto.Connection.Websocket.Tests.Server {
	[TestFixture]
	public class ClientTests {
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
			var objClient = new Websocket.Server.Client(objWebSocketBehavior);
			var objHandler = Substitute.For<ConnectionHandler>();

			objClient.Disconnected += objHandler;

			//Act
			objClient.Disconnect();

			//Assert
			objWebSocketBehavior.Received().OnCloseReceived -= Arg.Any<EventHandler<ICloseEventArgs>>();
			objWebSocketBehavior.Received().OnErrorReceived -= Arg.Any<EventHandler<IErrorEventArgs>>();
			objWebSocketBehavior.Received().OnMessageReceived -= Arg.Any<EventHandler<IMessageEventArgs>>();

			objHandler
				.Received(1)
				.Invoke(Arg.Is<IConnection.IConnection>(c => c.Equals(objClient)));
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
			objWebSocketBehavior.ID.Returns("MyClientID");

			//Act
			var objClient = new Websocket.Server.Client(objWebSocketBehavior);

			//Assert
			objWebSocketBehavior.Received().OnCloseReceived += Arg.Any<EventHandler<ICloseEventArgs>>();
			objWebSocketBehavior.Received().OnErrorReceived += Arg.Any<EventHandler<IErrorEventArgs>>();
			objWebSocketBehavior.Received().OnMessageReceived += Arg.Any<EventHandler<IMessageEventArgs>>();

			Assert.AreEqual("MyClientID", objClient.ID);
		}

		/// <summary>
		/// Tests the Disconnect event when the WebSocketBehavior.OnClose is raised
		/// This means the Close method is being called
		/// </summary>
		[Test]
		public void DisconnectedOnCloseTest() {
			//Setup
			var objWebSocketBehavior = Substitute.For<IWebSocketBehavior>();
			var objClient = new Websocket.Server.Client(objWebSocketBehavior);
			var objEventArgs = Substitute.For<ICloseEventArgs>();
			var objHandler = Substitute.For<ConnectionHandler>();

			objClient.Disconnected += objHandler;

			//Act
			objWebSocketBehavior.OnCloseReceived += Raise.Event<EventHandler<ICloseEventArgs>>(objClient, objEventArgs);

			//Assert
			objWebSocketBehavior.Received().OnCloseReceived -= Arg.Any<EventHandler<ICloseEventArgs>>();
			objWebSocketBehavior.Received().OnErrorReceived -= Arg.Any<EventHandler<IErrorEventArgs>>();
			objWebSocketBehavior.Received().OnMessageReceived -= Arg.Any<EventHandler<IMessageEventArgs>>();

			objHandler
				.Received(1)
				.Invoke(Arg.Is<IConnection.IConnection>(c => c.Equals(objClient)));

		}

		/// <summary>
		/// Tests the Disconnect event when the WebSocketBehavior.OnError is raised
		/// This means the Close method is being called
		/// </summary>
		[Test]
		public void DisconnectedOnErrorTest() {
			//Setup
			var objWebSocketBehavior = Substitute.For<IWebSocketBehavior>();
			var objClient = new Websocket.Server.Client(objWebSocketBehavior);
			var objEventArgs = Substitute.For<IErrorEventArgs>();
			var objHandler = Substitute.For<ConnectionHandler>();

			objClient.Disconnected += objHandler;

			//Act
			objWebSocketBehavior.OnErrorReceived += Raise.Event<EventHandler<IErrorEventArgs>>(objClient, objEventArgs);

			//Assert
			objWebSocketBehavior.Received().OnCloseReceived -= Arg.Any<EventHandler<ICloseEventArgs>>();
			objWebSocketBehavior.Received().OnErrorReceived -= Arg.Any<EventHandler<IErrorEventArgs>>();
			objWebSocketBehavior.Received().OnMessageReceived -= Arg.Any<EventHandler<IMessageEventArgs>>();

			objHandler
				.Received(1)
				.Invoke(Arg.Is<IConnection.IConnection>(c => c.Equals(objClient)));
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
			var objClient = new Websocket.Server.Client(objWebSocketBehavior);
			var objHandler = Substitute.For<DataHandler>();
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
			var objClient = new Websocket.Server.Client(objWebSocketBehavior);
			var objEventArgs = Substitute.For<IMessageEventArgs>();
			objEventArgs.IsBinary.Returns(false);
			objEventArgs.IsText.Returns(false);
			objEventArgs.IsPing.Returns(true);

			var eventReceived = false;
			objClient.Rx += delegate(IConnection.IConnection pConnection, byte[] pData) {
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
			var objClient = new Websocket.Server.Client(objWebSocketBehavior);
			var objHandler = Substitute.For<DataHandler>();
			var objEventArgs = Substitute.For<IMessageEventArgs>();
			objEventArgs.IsBinary.Returns(false);
			objEventArgs.IsText.Returns(true);
			objEventArgs.IsPing.Returns(false);
			objEventArgs.Data.Returns("MyString");

			objClient.Rx += objHandler;

			//Act
			objWebSocketBehavior.OnMessageReceived += Raise.Event<EventHandler<IMessageEventArgs>>(objClient, objEventArgs);

			//Assert
			objHandler
				.Received(1)
				.Invoke(
					Arg.Is<IConnection.IConnection>(c => c.Equals(objClient)), 
					Arg.Is<byte[]>(b => b.SequenceEqual(System.Text.Encoding.UTF8.GetBytes("MyString")))
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
			var objClient = new Websocket.Server.Client(objWebSocketBehavior);

			//Act
			objClient.Transmit(new byte[] { 1, 2, 3, 4 });
			System.Threading.Thread.Sleep(50); // wait a bit, data is being transmitted to IWebSocketBehavior on a separate thread

			//Assert
			objWebSocketBehavior.Received(1).Send(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })));
		}
	}
}