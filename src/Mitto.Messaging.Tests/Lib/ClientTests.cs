using NUnit.Framework;
using System;
using System.Linq;
using NSubstitute;
using Mitto.IMessaging;

namespace Mitto.Messaging.Tests {
	[TestFixture]
	public class ClientTests {
		/// <summary>
		/// Tests that when a request is made the request is routed to the IRequestManager
		/// </summary>
		[Test]
		public void RequestTest() {
			//Arrange
			var objRequestManager = Substitute.For<IRequestManager>();
			var objQueue = Substitute.For<IQueue.IQueue>();
			var objMessage = Substitute.For<IMessage>();
			Action<Response.ACK> objAction = r => { };

			//Act
			var objClient = new Client(objQueue, objRequestManager);
			objClient.Request(objMessage, objAction);
			
			//Assert
			objRequestManager.Received(1).Request<Response.ACK>(Arg.Any<IRequest>());
		}

		/// <summary>
		/// Tests that Transmitting data to the IQueue works
		/// This means that the IConverter will get the byte[] for the IMessage
		/// that is being transfered and that the IQueue.Transmit method will 
		/// be called with that binary data
		/// </summary>
		[Test]
		public void TransmitTest() {
			//Arrange
			var objConverter = Substitute.For<IMessageConverter>();
			var objRequestManager = Substitute.For<IRequestManager>();
			var objQueue = Substitute.For<IQueue.IQueue>();
			var objMessage = Substitute.For<IMessage>();

			objConverter.GetByteArray(Arg.Is(objMessage)).Returns(new byte[] { 1, 2, 3, 4, 5 });

			Config.Initialize(new Config.ConfigParams { MessageConverter = objConverter });

			//Act
			var objClient = new Client(objQueue, objRequestManager);
			objClient.Transmit(objMessage);

			//Assert
			objConverter.Received(1).GetByteArray(Arg.Is(objMessage));
			objQueue.Received(1).Transmit(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 0, 0, 1, 2, 3, 4, 5 })));
		}
	}
}
