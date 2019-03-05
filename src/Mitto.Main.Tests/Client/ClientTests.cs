using Mitto.IConnection;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;

namespace Mitto.Main.Tests.Client {
	[TestFixture]
	public class ClientTests {
		/// <summary>
		/// Tests the IConnection.IClient connect event
		/// This means that the Connected event is raised
		/// </summary>
		[Test]
		public void ClientConnectedTest() {
			//Arrange
			var objProvider = Substitute.For<IConnectionProvider>();
			var objConnection = Substitute.For<IClient>();
			var objHandler = Substitute.For<ClientConnectionHandler>();

			objProvider.CreateClient().Returns(objConnection);
			Config.Initialize(new Config.ConfigParams() {
				ConnectionProvider = objProvider
			});

			//Act
			var obj = new Mitto.Client();
			obj.Connected += objHandler;
			objConnection.Connected += Raise.Event<EventHandler<IClient>>(new object(), objConnection);

			//Assert
			objHandler
				.Received(1)
				.Invoke(Arg.Is(obj))
			;
		}

		/// <summary>
		/// Tests the IConnection.IClient data received event
		/// This means that the InternalQueue.Transmit is called with the 
		/// passed byte[]
		/// </summary>
		[Test]
		public void ClientDataReceivedTest() {
			//Arrange
			var objProvider = Substitute.For<IConnectionProvider>();
			var objQueueProvider = Substitute.For<IQueue.IQueueProvider>();

			var objConnection = Substitute.For<IClient>();
			var objQueue = Substitute.For<IQueue.IQueue>();

			objProvider.CreateClient().Returns(objConnection);
			objQueueProvider.Create().Returns(objQueue);

			Config.Initialize(new Config.ConfigParams() {
				ConnectionProvider = objProvider,
				QueueProvider = objQueueProvider

			});

			//Act
			var obj = new Mitto.Client();
			objConnection.Rx += Raise.Event<DataHandler>(Substitute.For<IConnection.IConnection>(), new byte[] { 1, 2, 3, 4, 5 });

			//Assert
			objQueue.Received(1).Transmit(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4, 5 })));
		}


		/// <summary>
		/// Tests the IConnection.IClient disconnect event
		/// This means that the Connection subscription handlers are removed
		/// and that the Disconnected event is called
		/// </summary>
		[Test]
		public void ClientDisconnectedTest() {
			//Arrange
			var objProvider = Substitute.For<IConnectionProvider>();
			var objQueueProvider = Substitute.For<IQueue.IQueueProvider>();

			var objConnection = Substitute.For<IClient>();
			var objHandler = Substitute.For<ClientConnectionHandler>();
			var objEventConnection = Substitute.For<IConnection.IConnection>();
			var objQueue = Substitute.For<IQueue.IQueue>();

			objProvider.CreateClient().Returns(objConnection);
			objQueueProvider.Create().Returns(objQueue);

			Config.Initialize(new Config.ConfigParams() {
				ConnectionProvider = objProvider,
				QueueProvider = objQueueProvider
			});

			//Act
			var obj = new Mitto.Client();
			obj.Disconnected += objHandler;
			objConnection.Disconnected += Raise.Event<EventHandler<IConnection.IConnection>>(new object(), objEventConnection);

			//Assert
			objConnection.Received(1).Rx -= Arg.Any<DataHandler>();
			objConnection.Received(1).Connected -= Arg.Any<EventHandler<IClient>>();
			objConnection.Received(1).Disconnected -= Arg.Any<EventHandler<IConnection.IConnection>>();
			objQueue.Received(1).Rx -= Arg.Any<Mitto.IQueue.DataHandler>();

			objHandler
				.Received(1)
				.Invoke(Arg.Is(obj))
			;
		}

		/// <summary>
		/// Tests the ConnectAsync method
		/// This means that the connectasync method is called on the 
		/// the Connection returned from the ConnectionProvider
		/// </summary>
		[Test]
		public void ConnectAsyncText() {
			//Arrange
			var objProvider = Substitute.For<IConnectionProvider>();
			var objConnection = Substitute.For<IClient>();
			var objParams = Substitute.For<IClientParams>();

			objProvider.CreateClient().Returns(objConnection);

			Config.Initialize(new Config.ConfigParams() {
				ConnectionProvider = objProvider
			});

			//Act
			var obj = new Mitto.Client();
			obj.ConnectAsync(objParams);

			//Assert
			objConnection.Received(1).ConnectAsync(Arg.Is(objParams));
		}

		/// <summary>
		/// Tests the Client creation
		/// This means that the IConnection is set as well as the IQueue
		/// The event handlers for connected, disconnected, data received are also attached
		/// </summary>
		[Test]
		public void CreateTest() {
			//Arrange
			var objProvider = Substitute.For<IConnectionProvider>();
			var objQueueProvider = Substitute.For<IQueue.IQueueProvider>();
			var objConnection = Substitute.For<IClient>();
			var objQueue = Substitute.For<IQueue.IQueue>();

			objProvider.CreateClient().Returns(objConnection);
			objQueueProvider.Create().Returns(objQueue);

			Config.Initialize(new Config.ConfigParams() {
				ConnectionProvider = objProvider,
				QueueProvider = objQueueProvider
			});

			//Act
			var obj = new Mitto.Client();

			//Assert
			objProvider.Received(1).CreateClient();
			objQueueProvider.Received(1).Create();

			objConnection.Received(1).Rx += Arg.Any<DataHandler>();
			objConnection.Received(1).Connected += Arg.Any<EventHandler<IClient>>();
			objConnection.Received(1).Disconnected += Arg.Any<EventHandler<IConnection.IConnection>>();
			objQueue.Received(1).Rx += Arg.Any<Mitto.IQueue.DataHandler>();
		}

		/// <summary>
		/// Test the Disconnect method
		/// This means that the close is called and the Disconnect on the IConnection.IClient
		/// Also verifies that the disconnect event is not fired
		/// </summary>
		[Test]
		public void DisconnectTest() {
			//Arrange
			var objProvider = Substitute.For<IConnectionProvider>();
			var objQueueProvider = Substitute.For<IQueue.IQueueProvider>();

			var objConnection = Substitute.For<IClient>();
			var objHandler = Substitute.For<ClientConnectionHandler>();
			var objQueue = Substitute.For<IQueue.IQueue>();

			objProvider.CreateClient().Returns(objConnection);
			objQueueProvider.Create().Returns(objQueue);

			Config.Initialize(new Config.ConfigParams() {
				ConnectionProvider = objProvider,
				QueueProvider = objQueueProvider
			});

			//Act
			var obj = new Mitto.Client();
			obj.Disconnected += objHandler;
			obj.Disconnect();

			//Assert
			objConnection.Received(1).Rx -= Arg.Any<DataHandler>();
			objConnection.Received(1).Connected -= Arg.Any<EventHandler<IClient>>();
			objConnection.Received(1).Disconnected -= Arg.Any<EventHandler<IConnection.IConnection>>();
			objQueue.Received(1).Rx -= Arg.Any<Mitto.IQueue.DataHandler>();

			objHandler
				.Received(1)
				.Invoke(Arg.Is(obj))
			;
		}

		/// <summary>
		/// Tests the data received event from the internal queue
		/// This means that the transmit method is called on the Client itself
		/// </summary>
		[Test]
		public void InternalDataReceived() {
			//Arrange
			var objProvider = Substitute.For<IConnectionProvider>();
			var objQueueProvider = Substitute.For<IQueue.IQueueProvider>();

			var objConnection = Substitute.For<IClient>();
			var objHandler = Substitute.For<Mitto.IQueue.DataHandler>();
			var objQueue = Substitute.For<IQueue.IQueue>();

			objProvider.CreateClient().Returns(objConnection);
			objQueueProvider.Create().Returns(objQueue);

			Config.Initialize(new Config.ConfigParams() {
				ConnectionProvider = objProvider,
				QueueProvider = objQueueProvider
			});

			//Act
			var obj = new Mitto.Client();
			obj.Rx += objHandler;
			objQueue.Rx += Raise.Event<Mitto.IQueue.DataHandler>(new byte[] { 1, 2, 3, 4, 5 });

			//Assert
			objConnection.Received(1).Transmit(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4, 5 })));
			objHandler
				.Received(1)
				.Invoke(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4, 5 })));
			;
		}

		/// <summary>
		/// Tests the Receive method
		/// This means that when the Client receives data, the Rx event is called with that data
		/// </summary>
		[Test]
		public void ReceiveTest() {
			//Arrange
			var objProvider = Substitute.For<IConnectionProvider>();
			var objQueueProvider = Substitute.For<IQueue.IQueueProvider>();
			var objHandler = Substitute.For<Mitto.IQueue.DataHandler>();

			Config.Initialize(new Config.ConfigParams() {
				ConnectionProvider = objProvider,
				QueueProvider = objQueueProvider
			});

			//Act
			var obj = new Mitto.Client();
			obj.Rx += objHandler;
			obj.Receive(new byte[] { 1, 2, 3, 4, 5 });

			//Assert
			objHandler
				.Received(1)
				.Invoke(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4, 5 })));
			;
		}

		/// <summary>
		/// Tests the Request method
		/// This means that the Request is passed onto the MessageProcesser
		/// </summary>
		[Test]
		public void RequestTest() {
			//Arrange
			var objProvider = Substitute.For<IConnectionProvider>();
			var objQueueProvider = Substitute.For<IQueue.IQueueProvider>();
			var objProcessor = Substitute.For<IMessaging.IMessageProcessor>();

			Config.Initialize(new Config.ConfigParams() {
				ConnectionProvider = objProvider,
				QueueProvider = objQueueProvider,
				MessageProcessor = objProcessor
			});

			var objRequestMessage = Substitute.For<IMessaging.IRequestMessage>();
			var objAction = Substitute.For<Action<IMessaging.IResponseMessage>>();

			//Act
			var obj = new Mitto.Client();
			obj.Request(objRequestMessage, objAction);

			//Assert
			objProcessor.Received(1).Request(Arg.Is(obj), Arg.Is(objRequestMessage), Arg.Is(objAction));
		}

		/// <summary>
		/// Tests the Respond method
		/// This means that the byte[] is passed onto the 
		/// Transmit method from the internal Queue
		/// </summary>
		[Test]
		public void RespondTest() {
			//Arrange
			var objProvider = Substitute.For<IConnectionProvider>();
			var objQueueProvider = Substitute.For<IQueue.IQueueProvider>();
			var objQueue = Substitute.For<IQueue.IQueue>();

			objQueueProvider.Create().Returns(objQueue);

			Config.Initialize(new Config.ConfigParams() {
				ConnectionProvider = objProvider,
				QueueProvider = objQueueProvider,
			});

			//Act
			var obj = new Mitto.Client();
			obj.Respond(new byte[] { 1, 2, 3, 4, 5 });

			//Assert
			objQueue.Received(1).Transmit(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4, 5 })));
		}

		/// <summary>
		/// Tests the transmit method
		/// This means that the IConnection.Transmit is called with the byte[] 
		/// and that the Rx event is triggered
		/// </summary>
		[Test]
		public void TransmitTest() {
			//Arrange
			var objProvider = Substitute.For<IConnectionProvider>();
			var objQueueProvider = Substitute.For<IQueue.IQueueProvider>();
			var objHandler = Substitute.For<Mitto.IQueue.DataHandler>();

			//var objQueue = Substitute.For<IQueue.IQueue>();
			//objQueueProvider.Create().Returns(objQueue);
			var objConnection = Substitute.For<IClient>();
			objProvider.CreateClient().Returns(objConnection);

			Config.Initialize(new Config.ConfigParams() {
				ConnectionProvider = objProvider,
				QueueProvider = objQueueProvider
			});

			//Act
			var obj = new Mitto.Client();
			obj.Rx += objHandler;
			obj.Transmit(new byte[] { 1, 2, 3, 4, 5 });

			//Assert
			objConnection.Received(1).Transmit(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4, 5 })));
			objHandler
				.Received(1)
				.Invoke(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4, 5 })));
			;
		}
	}
}
