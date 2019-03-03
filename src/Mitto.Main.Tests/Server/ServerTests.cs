using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Mitto.Main.Tests.Server {
	[TestFixture]
	public class ServerTests {
		/// <summary>
		/// Tests the Server constructor/creation
		/// This means that the ConnectionFactory.CreateServer is called
		/// </summary>
		[Test]
		public void CreateTest() {
			//Arrange
			var objConnectionProvider = Substitute.For<IConnection.IConnectionProvider>();

			Config.Initialize(new Config.ConfigParams() {
				ConnectionProvider = objConnectionProvider
			});

			//Act
			var obj = new Mitto.Server();

			//Assert
			objConnectionProvider.Received(1).CreateServer();
		}

		/// <summary>
		/// Tests starting the server without ssl
		/// This means that the Start on the IServer is called
		/// with the parameters for certificate and its password empty
		/// </summary>
		[Test]
		public void StartInSecureStartTest() {
			//Arrange
			var objConnectionProvider = Substitute.For<IConnection.IConnectionProvider>();
			var objAction = Substitute.For<Action<ClientConnection>>();
			var objConnection = Substitute.For<IConnection.IServer>();

			objConnectionProvider.CreateServer().Returns(objConnection);

			Config.Initialize(new Config.ConfigParams() {
				ConnectionProvider = objConnectionProvider
			});

			//Act
			var obj = new Mitto.Server();
			obj.Start(IPAddress.Any, 8080, objAction);

			//Assert
			objConnection.Received(1).Start(Arg.Any<IPAddress>(), 8080, Arg.Is(String.Empty), Arg.Is(String.Empty), Arg.Any<Action<IConnection.IClientConnection>>());
		}

		/// <summary>
		/// Tests starting the server 
		/// </summary>
		[Test]
		public void StartSecureTest() {
			//Arrange
			var objConnectionProvider = Substitute.For<IConnection.IConnectionProvider>();
			var objAction = Substitute.For<Action<ClientConnection>>();
			var objConnection = Substitute.For<IConnection.IServer>();

			objConnectionProvider.CreateServer().Returns(objConnection);

			Config.Initialize(new Config.ConfigParams() {
				ConnectionProvider = objConnectionProvider
			});

			//Act
			var obj = new Mitto.Server();
			obj.Start(IPAddress.Any, 8080, "Path", "Password", objAction);

			//Assert
			objConnection.Received(1).Start(Arg.Any<IPAddress>(), 8080, Arg.Is("Path"), Arg.Is("Password"), Arg.Any<Action<IConnection.IClientConnection>>());
		}


	}
}
