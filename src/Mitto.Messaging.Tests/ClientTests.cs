using NUnit.Framework;
using System;
using System.Linq;
using NSubstitute;
using Mitto.IMessaging;
using Mitto.IRouting;
using System.Collections.Generic;
using System.Text;

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
			var objQueue = Substitute.For<IRouter>();
			var objMessage = Substitute.For<IRequestMessage>();
			Action<Response.ACKResponse> objAction = r => { };

			//Act
			var objClient = new Client(objQueue, objRequestManager);
			objClient.Request(objMessage, objAction);

			//Assert
			objRequestManager.Received(1).Request<Response.ACKResponse>(Arg.Any<IRequest>());
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
			var objQueue = Substitute.For<IRouter>();
			var objMessage = Substitute.For<IMessage>();

			Config.Initialize(new Config.ConfigParams { MessageConverter = objConverter });

			//Act
			var objClient = new Client(objQueue, objRequestManager);
			objClient.Transmit(objMessage);

			//Assert
			objConverter.Received(1).GetByteArray(Arg.Is(objMessage));
			objQueue.Received(1).Transmit(Arg.Any<byte[]>());
		}
	}
}
