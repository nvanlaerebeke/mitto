using NUnit.Framework;
using NSubstitute;
using System;
using System.Linq;
using Mitto.IMessaging;

namespace Mitto.Messaging.Tests {
	[TestFixture]
	public class RequestManagerTests {
		/// <summary>
		/// Tests what happens when a request is made 
		/// This means that request is added to the dictionary and 
		/// the Transmit method is called on IRequest 
		/// </summary>
		[Test]
		public void RequestTest() {
			//Arrange
			var objRequest = Substitute.For<IRequest>();

			//Act
			var obj = new RequestManager();
			obj.Request<IResponseMessage>(objRequest);

			//Assert
			objRequest.Received(1).RequestTimedOut += Arg.Any<EventHandler<IRequest>>();
			objRequest.Received(1).Transmit();
		}


		/// <summary>
		/// Tests what happens when SetResponse is called
		/// This means that SetResponse is called on the IRequest
		/// </summary>
		[Test]
		public void SetResponseTest() {
			//Arrange
			var objRequest = Substitute.For<IRequest>();
			var objResponse = Substitute.For<IResponseMessage>();

			//Act
			var obj = new RequestManager();
			obj.Request<IResponseMessage>(objRequest);
			obj.SetResponse(objResponse);

			//Assert
			objRequest.Received(1).SetResponse(Arg.Is(objResponse));
			objRequest.Received(1).RequestTimedOut -= Arg.Any<EventHandler<IRequest>>();
		}

		[Test]
		public void TimedOutTest() {
			//Arrange
			var objProvider = Substitute.For<IMessageProvider>();
			var objRequest = Substitute.For<IRequest>();
			var objResponse = Substitute.For<IResponseMessage>();
            var objResponseStatus = new ResponseStatus(ResponseState.TimeOut);
            
			objRequest.Message.Returns(Substitute.For<IRequestMessage>());
            objProvider.GetResponseMessage(Arg.Is(objRequest.Message), Arg.Any<ResponseStatus>()).Returns(objResponse);

            Config.Initialize(new Config.ConfigParams() {
				MessageProvider = objProvider
			});

			//Act
			var obj = new RequestManager();
			obj.Request<IResponseMessage>(objRequest);
			objRequest.RequestTimedOut += Raise.Event<EventHandler<IRequest>>(this, objRequest);

			//Assert
			objProvider.Received(1).GetResponseMessage(Arg.Is(objRequest.Message), Arg.Is<ResponseStatus>(r => r.State == ResponseState.TimeOut));
			objRequest.Received(1).RequestTimedOut += Arg.Any<EventHandler<IRequest>>();
			objRequest.Received(1).RequestTimedOut -= Arg.Any<EventHandler<IRequest>>();
			objRequest.Received(1).SetResponse(Arg.Is(objResponse));
		}


		/// <summary>
		/// Tests the GetStatus method where a Busy state is expected
		/// This means that a Request is made and then the GetStatus is called
		/// It's expected that the Status would be busy
		/// </summary>
		[Test]
		public void GetStatusBusyTest() {
			//Arrange
			var objRequest = Substitute.For<IRequest>();
			var objResponse = Substitute.For<IResponseMessage>();
			objRequest.Message.ID.Returns("MyID");

			//Act
			var obj = new RequestManager();
			obj.Request<IResponseMessage>(objRequest);

			//Assert
			Assert.AreEqual(MessageStatusType.Queued, obj.GetStatus(objRequest.Message.ID));
		}

		/// <summary>
		/// Tests the GetStatus method where an UnKnwon state is expected
		/// This means that a GetStatus is done for a IMessage.ID that 
		/// was never started
		/// </summary>
		[Test]
		public void GetStatusUnKnownTest() {
			//Arrange
			//Act
			var obj = new RequestManager();
			//Assert
			Assert.AreEqual(MessageStatusType.UnKnown, obj.GetStatus("MyID"));
		}
	}
}