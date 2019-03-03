using Mitto.Connection.Websocket.Server;
using Mitto.IConnection;
using NSubstitute;
using NUnit.Framework;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Mitto.Connection.Websocket.Tests.Server {
	[TestFixture]
	public class WebsocketServerTests {

		/// <summary>
		/// Tests starting an insecure WebsocketServer 
		/// </summary>
		[Test]
		public void StartInSecureTest() {
			//Arrange
			var objWebSocketServer = Substitute.For<IWebSocketServer>();
			var objAction = Substitute.For<Action<IClientConnection>>();

			//Act
			var objServer = new WebsocketServer(objWebSocketServer);
			objServer.Start(new ServerParams(IPAddress.Parse("127.0.0.1"), 80), objAction);
			
			//Assert
			objWebSocketServer.Received(1).Start(
				Arg.Is<IPAddress>(ip => ip.Equals(IPAddress.Parse("127.0.0.1"))),
				Arg.Is<int>(p => p.Equals(80))
			);
		}


		/// <summary>
		/// Tests starting a secure WebsocketServer
		/// This means that a Start is called with a certificate and it's expected that the 
		/// IWebSocketServer start method is called with ip/port/cert
		/// </summary>
		[Test]
		public void StartSecureTest() {
			//Arrange
			var objWebSocketServer = Substitute.For<IWebSocketServer>();
			var objAction = Substitute.For<Action<IClientConnection>>();

			var cert = "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";
			var file = Path.GetTempFileName();
			File.WriteAllBytes(file, Convert.FromBase64String(cert));

			//Act
			var objServer = new WebsocketServer(objWebSocketServer);
			objServer.Start(new ServerParams(IPAddress.Parse("127.0.0.1"), 443, file, ""), objAction);

			//Assert
			objWebSocketServer.Received(1).Start(
				Arg.Is<IPAddress>(ip => ip.Equals(IPAddress.Parse("127.0.0.1"))),
				Arg.Is<int>(p => p.Equals(443)),
				Arg.Is<X509Certificate2>(c => c != null) //could test the raw data?
			);
		}

		[Test]
		public void ClientConnectedTest() {
			//Arrange
			var objWebSocketServer = Substitute.For<IWebSocketServer>();
			var objAction = Substitute.For<Action<IClientConnection>>();
			var objEventArgs = Substitute.For<IWebSocketBehavior>();
			var objServer = new WebsocketServer(objWebSocketServer);

			//Act
			var objParams = new ServerParams(IPAddress.Any, 80);
			objServer.Start(objParams, objAction);
			objWebSocketServer.ClientConnected += Raise.Event<EventHandler<IWebSocketBehavior>>(objWebSocketServer, objEventArgs);

			//Assert
			objAction.Received(1).Invoke(Arg.Any<IClientConnection>());
		}

		/// <summary>
		/// Test the server stop 
		/// This means the Stop is called on the IWebSocketServer
		/// </summary>
		[Test]
		public void StopTest() {
			//Setup
			var objWebSocketServer = Substitute.For<IWebSocketServer>();
			var objAction = Substitute.For<Action<IClientConnection>>();
			var objEventArgs = Substitute.For<IWebSocketBehavior>();
			var objServer = new WebsocketServer(objWebSocketServer);

			//Act
			var objParams = new ServerParams(IPAddress.Any, 80);
			objServer.Start(objParams, objAction);
			objWebSocketServer.ClientConnected += Raise.Event<EventHandler<IWebSocketBehavior>>(objWebSocketServer, objEventArgs);
			objServer.Stop();
			objWebSocketServer.ClientConnected += Raise.Event<EventHandler<IWebSocketBehavior>>(objWebSocketServer, objEventArgs);

			//Assert
			objAction.Received(1).Invoke(Arg.Any<IClientConnection>());
			objWebSocketServer.Received(1).Stop();
		}
	}
}