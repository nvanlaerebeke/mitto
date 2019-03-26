using NUnit.Framework;
using NSubstitute;
using System.Linq;
using System;
using Mitto.IConnection;
using Mitto.IMessaging;
using Mitto.IRouting;

namespace Mitto.Routing.PassThrough.Tests {
	[TestFixture()]
	public class PassThroughRouterTests {
		/// <summary>
		/// Tests the Close method
		/// This means that when close is called the IClientConnetion.Rx
		/// event handler is removed
		/// </summary>
		[Test]
		public void CloseTest() {
			//Arrange
			var objConnection = Substitute.For<IClientConnection>();

			//Act
			var obj = new PassThroughRouter(objConnection);
			obj.Close();

			//Assert
			objConnection.Rx -= Arg.Any<EventHandler<byte[]>>();
		}

		/// <summary>
		/// Tests the PassThroughRouter creation
		/// This means that the IClientConnection.Rx event handler is attached
		/// </summary>
		[Test]
		public void CreateTest() {
			//Arrange
			var objConnection = Substitute.For<IClientConnection>();

			//Act
			var obj = new PassThroughRouter(objConnection);

			//Assert
			objConnection.Rx += Arg.Any<EventHandler<byte[]>>();
		}

		/// <summary>
		/// Tests the IClientConnection.Rx event handler
		/// 
		/// This means that when the IClientConnection.Rx is raised, the 
		/// IMessageProcessor is called with the correct data
		/// </summary>
		[Test]
		public void DataReceivedTest() {
			//Arrange
			var objMessageProcessor = Substitute.For<IMessageProcessor>();
			var objConnection = Substitute.For<IClientConnection>();
			var objRoutingFrame = new RoutingFrame(RoutingFrameType.Messaging, "MyRequestID", "MySourceID", "MyDestinationID", new byte[] { 1, 2, 3, 4 });

			Config.Initialize(new Config.ConfigParams(){
				MessageProcessor = objMessageProcessor
			});

			//Act
			var obj = new PassThroughRouter(objConnection);
			objConnection.Rx += Raise.Event<EventHandler<byte[]>>(objConnection, objRoutingFrame.GetBytes());

			//Assert
			objMessageProcessor.Received(1).Process(Arg.Is(obj), Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })));
		}

		/// <summary>
		/// Tests the Router transmit method
		/// This means that the binary data is passed alone to the IClientConnection
		/// </summary>
		[Test]
		public void TransmitTest() {
			//Arrange
			var objConnection = Substitute.For<IClientConnection>();

			//Act
			new PassThroughRouter(objConnection).Transmit(new byte[] { 1, 2, 3, 4 });

			//Assert
			objConnection.Received(1).Transmit(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })));
		}
	}
}