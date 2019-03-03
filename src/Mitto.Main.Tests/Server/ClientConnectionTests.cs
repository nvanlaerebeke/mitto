using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mitto.Main.Tests.Server {
	[TestFixture]
	public class ClientConnectionTests {
		/// <summary>
		/// Tests the DataReceived event on the IConnection
		/// This means that the InternalQueue.Transmit is called 
		/// with the given byte[]
		/// </summary>
		[Test]
		public void ConnectionDataReceivedTest() {
			//Arrange
			var objQueueProvider = Substitute.For<IQueue.IQueueProvider>();
			var objConnection = Substitute.For<IConnection.IClientConnection>();
			var objBaseConnection = Substitute.For<IConnection.IClientConnection>();
			var objQueue = Substitute.For<IQueue.IQueue>();

			objQueueProvider.Create().Returns(objQueue);

			Config.Initialize(new Config.ConfigParams() {
				QueueProvider = objQueueProvider
			});

			//Act
			var obj = new ClientConnection(objConnection);
			objConnection.Rx += Raise.Event<IConnection.DataHandler>(objBaseConnection, new byte[] { 1, 2, 3, 4, 5 });

			//Assert
			objQueue.Received(1).Transmit(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4, 5 })));
		}

		/// <summary>
		/// Tests the IClientConnection disconnect event
		/// This means that the event handers are removed and 
		/// the Disconnected event is raised
		///
		/// </summary>
		[Test]
		public void ConnectionDisconnectedTest() {
			//Arrange
			var objQueueProvider = Substitute.For<IQueue.IQueueProvider>();
			var objConnection = Substitute.For<IConnection.IClientConnection>();
			var objQueue = Substitute.For<IQueue.IQueue>();
			var objHandler = Substitute.For<EventHandler<ClientConnection>>();
			var objEventConnection = Substitute.For<IConnection.IConnection>();

			objQueueProvider.Create().Returns(objQueue);

			Config.Initialize(new Config.ConfigParams() {
				QueueProvider = objQueueProvider
			});

			//Act
			var obj = new ClientConnection(objConnection);
			obj.Disconnected += objHandler;
			objConnection.Disconnected += Raise.Event<EventHandler<IConnection.IConnection>>(new object(), objEventConnection);

			//Assert
			objConnection.Received(1).Disconnected -= Arg.Any<EventHandler<IConnection.IConnection>>();
			objConnection.Received(1).Rx -= Arg.Any<IConnection.DataHandler>();
			objQueue.Received(1).Rx -= Arg.Any<IQueue.DataHandler>();
			objHandler.Received(1).Invoke(Arg.Any<IConnection.IConnection>(), Arg.Is(obj));
		}

		/// <summary>
		/// Tests the ClientConnection constructor
		/// This means that the eventhandlers are attached to the IConnection
		/// and the Internal Queue
		/// </summary>
		[Test]
		public void CreateTest() {
			//Arrange
			var objQueueProvider = Substitute.For<IQueue.IQueueProvider>();
			var objConnection = Substitute.For<IConnection.IClientConnection>();
			var objQueue = Substitute.For<IQueue.IQueue>();

			objQueueProvider.Create().Returns(objQueue);

			Config.Initialize(new Config.ConfigParams() {
				QueueProvider = objQueueProvider
			});

			//Act
			var obj = new ClientConnection(objConnection);

			//Assert
			objQueueProvider.Received(1).Create();
			objConnection.Received(1).Disconnected += Arg.Any<EventHandler<IConnection.IConnection>>();
			objConnection.Received(1).Rx += Arg.Any<IConnection.DataHandler>();
			objQueue.Received(1).Rx += Arg.Any<IQueue.DataHandler>();
		}

		/// <summary>
		/// Tests the InternalQueue data received event
		/// This means that the IConnection.Transmit is called with said data
		/// </summary>
		[Test]
		public void InternalQueueDataReceivedTest() {
			//Arrange
			var objQueueProvider = Substitute.For<IQueue.IQueueProvider>();
			var objConnection = Substitute.For<IConnection.IClientConnection>();
			var objQueue = Substitute.For<IQueue.IQueue>();

			objQueueProvider.Create().Returns(objQueue);

			Config.Initialize(new Config.ConfigParams() {
				QueueProvider = objQueueProvider
			});

			//Act
			var obj = new ClientConnection(objConnection);
			objQueue.Rx += Raise.Event<IQueue.DataHandler>(new byte[] { 1, 2, 3, 4, 5 });

			//Assert
			objConnection.Received(1).Transmit(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4, 5 })));
		}
	}
}