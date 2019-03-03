using Mitto.IMessaging;
using NSubstitute;
using NUnit.Framework;
using System;
using Mitto.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mitto.Messaging.Tests {
	[TestFixture]
	public class RequestTests {

		/// <summary>
		/// Tests the UnResponsive event handler
		/// This means that the RequestTimedOut event is raised
		/// </summary>
		[Test]
		public void UnResponsiveTest() {
			//Arrange
			var objClient = Substitute.For<IClient>();
			var objMessage = Substitute.For<IRequestMessage>();
			var objAction = Substitute.For<Action<Response.MessageStatusResponse>>();
			var objKeepAliveMonitor = Substitute.For<IKeepAliveMonitor>();
			var objHandler = Substitute.For<EventHandler<IRequest>>();

			//Act
			var obj = new Request<Response.MessageStatusResponse>(objClient, objMessage, objAction, objKeepAliveMonitor);
			obj.RequestTimedOut += objHandler;
			objKeepAliveMonitor.UnResponsive += Raise.Event<EventHandler>(new object(), new EventArgs());

			//Assert
			objKeepAliveMonitor.Received(1).Stop();
			objHandler.Received(1).Invoke(
				Arg.Any<object>(),
				Arg.Is(obj)
			);
		}

		/// <summary>
		/// Tests the request timeout
		/// This means that when the timeout is triggered the countdown for a failed
		/// request is started and a request is made to get the status.
		/// In this test it will be returned that the IRequest is still being processed
		/// </summary>
		[Test]
		public void ActionTimeOutTest() {
			//Arrange
			var objClient = Substitute.For<IClient>();
			var objMessage = Substitute.For<IRequestMessage>();
			var objAction = Substitute.For<Action<Response.MessageStatusResponse>>();
			var objKeepAliveMonitor = Substitute.For<IKeepAliveMonitor>();

			objMessage.ID.Returns("MyID");

			//Act
			var obj = new Request<Response.MessageStatusResponse>(objClient, objMessage, objAction, objKeepAliveMonitor);
			objKeepAliveMonitor.TimeOut += Raise.EventWith(new object(), new EventArgs());

			//Assert
			objKeepAliveMonitor.Received(1).StartCountDown();
			objClient.Received(1).Request(Arg.Is<Request.MessageStatusRequest>(m => m.RequestID.Equals(objMessage.ID)), Arg.Any<Action<Response.MessageStatusResponse>>());
		}

		/// <summary>
		/// Tests the SetResponse method
		/// This means that the passed action is invoked with the IResponseMessage
		/// </summary>
		[Test]
		public void SetResponseTest() {
			//Arrange
			var blnActionCalled = false;

			var objClient = Substitute.For<IClient>();
			var objMessage = Substitute.For<IRequestMessage>();
			var objAction = new Action<IResponseMessage>(r => {
				blnActionCalled = true;
			});
			var objResponse = Substitute.For<IResponseMessage>();

			//Act
			var obj = new Request<IResponseMessage>(objClient, objMessage, objAction);
			obj.SetResponse(objResponse);

			//Assert
			Assert.IsTrue(blnActionCalled);
		}


		/// <summary>
		/// Test transmitting the request
		/// This means the IClient.Transmit(IMessage) is called
		/// </summary>
		[Test]
		public void TransmitTest() {
			//Arrange
			var objClient = Substitute.For<IClient>();
			var objMessage = Substitute.For<IRequestMessage>();
			var objAction = Substitute.For<Action<IResponseMessage>>();

			//Act
			var obj = new Request<IResponseMessage>(objClient, objMessage, objAction);
			obj.Transmit();

			//Assert
			objClient.Received(1).Transmit(Arg.Is(objMessage));
		}
	}
}
