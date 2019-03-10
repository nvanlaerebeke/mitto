using Mitto.IConnection;
using Mitto.IRouting;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;

namespace Mitto.Main.Tests.Server {
	[TestFixture]
	public class ClientConnectionTests {
		/// <summary>
		/// Tests the IClientConnection disconnect event
		/// This means that the event handers are removed and 
		/// the Disconnected event is raised
		/// </summary>
		[Test]
		public void ConnectionDisconnectedTest() {
			//Arrange
			var objRouterProvider = Substitute.For<IRouterProvider>();
			var objConnection = Substitute.For<IClientConnection>();
			var objRouter = Substitute.For<IRouter>();
			var objHandler = Substitute.For<EventHandler<ClientConnection>>();

			objRouterProvider.Create(Arg.Is(objConnection)).Returns(objRouter);

			Config.Initialize(new Config.ConfigParams() {
				RouterProvider = objRouterProvider
			});

			//Act
			var obj = new ClientConnection(objConnection);
			obj.Disconnected += objHandler;
			objConnection.Disconnected += Raise.Event<EventHandler>(objConnection, new EventArgs());

			//Assert
			objConnection.Received(1).Disconnected -= Arg.Any<EventHandler>();
			objHandler.Received(1).Invoke(Arg.Is(obj), Arg.Is(obj));
		}

		/// <summary>
		/// Tests the ClientConnection constructor
		/// This means that the eventhandlers are attached to the IConnection
		/// and the router is created
		/// </summary>
		[Test]
		public void CreateTest() {
			//Arrange
			var objRouterProvider = Substitute.For<IRouterProvider>();
			var objConnection = Substitute.For<IClientConnection>();

			Config.Initialize(new Config.ConfigParams() {
				RouterProvider = objRouterProvider
			});

			//Act
			var obj = new ClientConnection(objConnection);

			//Assert
			objRouterProvider.Received(1).Create(Arg.Is(objConnection));
			objConnection.Received(1).Disconnected += Arg.Any<EventHandler>();
		}
	}
}