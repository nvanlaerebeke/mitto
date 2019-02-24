using Mitto.Connection.Websocket.Client;
using Mitto.IConnection;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using WebSocketSharp;

namespace Mitto.Connection.Websocket.Tests.Client {
	[TestFixture]
	public class WebsocketClientTests {

		/// <summary>
		/// Tests the close function
		/// This means verifying that all event subscriptions are gone
		/// and the disconnect  event was called
		/// </summary>
		[Test]
		public void DisconnectTest() {
			//Arrange
			var objWebSocketClient = Substitute.For<IWebSocketClient>();
			var objClient = new WebsocketClient(objWebSocketClient);
			var objHandler = Substitute.For<ConnectionHandler>();
			objClient.Disconnected += objHandler;

			//Act
			objClient.ConnectAsync("hostname", 80, false);
			objClient.Disconnect();
			
			//Assert
			objWebSocketClient.Received().OnOpen -= Arg.Any<EventHandler>();
			objWebSocketClient.Received().OnClose -= Arg.Any<EventHandler<ICloseEventArgs>>();
			objWebSocketClient.Received().OnError -= Arg.Any<EventHandler<IErrorEventArgs>>();
			objWebSocketClient.Received().OnMessage -= Arg.Any<EventHandler<IMessageEventArgs>>();
			objHandler
				.Received(1)
				.Invoke(Arg.Is<IConnection.IConnection>(c => c.Equals(objClient)))
			;
		}

		/// <summary>
		/// Tests the ConnectAsync method
		/// This means checking if if the event handlers were added on IWebSocketClient
		/// </summary>
		[Test]
		public void ConnectAsyncTest() {
			var objWebSocketClient = Substitute.For<IWebSocketClient>();

			var objClient = new WebsocketClient(objWebSocketClient);
			objClient.ConnectAsync("hostname", 80, false);

			//checks event subscriptions
			objWebSocketClient.Received().OnOpen += Arg.Any<EventHandler>();
			objWebSocketClient.Received().OnClose += Arg.Any<EventHandler<ICloseEventArgs>>();
			objWebSocketClient.Received().OnError += Arg.Any<EventHandler<IErrorEventArgs>>();
			objWebSocketClient.Received().OnMessage += Arg.Any<EventHandler<IMessageEventArgs>>();
		}

		/// <summary>
		/// Tests the connected event
		/// This means that after OnOpen is triggered on IWebSocketClient a Connected event
		/// is expected for the WebsocketClient
		/// </summary>
		[Test]
		public void ConnectedTest() {
			var objWebSocketClient = Substitute.For<IWebSocketClient>();
			var objClient = new WebsocketClient(objWebSocketClient);
			var handler = Substitute.For<ConnectionHandler>();

			objClient.Connected += handler;
			objClient.ConnectAsync("hostname", 80, false);
			objWebSocketClient.Received().OnOpen += Raise.EventWith(objWebSocketClient, new EventArgs());

			handler
				.Received(1)
				.Invoke(Arg.Is<IConnection.IConnection>(c => c.Equals(objClient)))
			;
		}

		/// <summary>
		/// Test the constructing of a WebsocketClient
		/// This means that the ID is set to a guid
		/// </summary>
		[Test]
		public void CreateTest() {
			var objWebSocketClient = Substitute.For<WebSocketClientWrapper>();
			var objClient = new WebsocketClient(objWebSocketClient);

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
			var objWebSocketClient = Substitute.For<IWebSocketClient>();
			objWebSocketClient.ReadyState.Returns(state);

			var objClient = new WebsocketClient(objWebSocketClient);
			var eventArgs = Substitute.For<ICloseEventArgs>();
			var handler = Substitute.For<ConnectionHandler>();

			objClient.Disconnected += handler;
			objClient.ConnectAsync("hostname", 80, false);
			objWebSocketClient.OnClose += Raise.Event<EventHandler<ICloseEventArgs>>(objClient, eventArgs);

			objWebSocketClient.Received().OnOpen -= Arg.Any<EventHandler>();
			objWebSocketClient.Received().OnClose -= Arg.Any<EventHandler<ICloseEventArgs>>();
			objWebSocketClient.Received().OnError -= Arg.Any<EventHandler<IErrorEventArgs>>();
			objWebSocketClient.Received().OnMessage -= Arg.Any<EventHandler<IMessageEventArgs>>();

			objWebSocketClient.Received((expectclose) ? 1 : 0).Close();

			handler
				.Received(1)
				.Invoke(Arg.Is<IConnection.IConnection>(c => c.Equals(objClient)))
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
			var objWebSocketClient = Substitute.For<IWebSocketClient>();
			objWebSocketClient.ReadyState.Returns(state);

			var objClient = new WebsocketClient(objWebSocketClient);
			var eventArgs = Substitute.For<IErrorEventArgs>();
			var handler = Substitute.For<ConnectionHandler>();

			objClient.Disconnected += handler;
			objClient.ConnectAsync("hostname", 80, false);
			objWebSocketClient.OnError += Raise.Event<EventHandler<IErrorEventArgs>>(objClient, eventArgs);

			objWebSocketClient.Received().OnOpen -= Arg.Any<EventHandler>();
			objWebSocketClient.Received().OnClose -= Arg.Any<EventHandler<ICloseEventArgs>>();
			objWebSocketClient.Received().OnError -= Arg.Any<EventHandler<IErrorEventArgs>>();
			objWebSocketClient.Received().OnMessage -= Arg.Any<EventHandler<IMessageEventArgs>>();

			objWebSocketClient.Received((expectclose) ? 1 : 0).Close();

			handler
				.Received(1)
				.Invoke(Arg.Is<IConnection.IConnection>(c => c.Equals(objClient)))
			;
		}

		/// <summary>
		/// Tests the message received event (Rx)
		/// This means that when OnMessage is triggered an Rx event with the binary data is expected
		/// </summary>
		[Test]
		public void RxTest() {
			var objWebSocketClient = Substitute.For<IWebSocketClient>();
			var objClient = new WebsocketClient(objWebSocketClient);
			var handler = Substitute.For<DataHandler>();

			objClient.Rx += handler;
			objClient.ConnectAsync("hostname", 80, false);
			var eventArgs = Substitute.For<IMessageEventArgs>();
			eventArgs.RawData.Returns(new byte[] { 1, 2, 3, 4 });
			objWebSocketClient.OnMessage += Raise.Event<EventHandler<IMessageEventArgs>>(objClient, eventArgs);

			handler
				.Received(1)
				.Invoke(Arg.Is<IConnection.IConnection>(m => m.Equals(objClient)), Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })))
			;
		}

		/// <summary>
		/// Tests the Transmit method
		/// This means that when binary data is passed to the Transmit method a call on the IWebSockClient.Send
		/// is expected in the very near future (sender queue = different thread)
		/// </summary>
		[Test]
		public void TransmitTest() {
			var objWebSocketClient = Substitute.For<IWebSocketClient>();
			var objClient = new WebsocketClient(objWebSocketClient);
			objClient.ConnectAsync("localhost", 80, false);
			objWebSocketClient.Received().OnOpen += Raise.EventWith(objWebSocketClient, new EventArgs());

			//Act
			objClient.Transmit(new byte[] { 1, 2, 3, 4 });
			Thread.Sleep(50); //-- wait a bit so the sender queue can call the IWebSocketClient.Send() method
			
			//Assert
			objWebSocketClient.Received(1).Send(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })));
		}
	}
}
