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
		/// This means that request is added to the dictionary and the Transmit method is 
		/// called on the IQueue.IQueue with the byte[] data provided by the IMessageProvider
		/// </summary>
		[Test]
		public void RequestTest() {
			//Arrange
			var obj = new RequestManager();
			var objProvider = Substitute.For<IMessageProvider>();
			var objClient = Substitute.For<IQueue.IQueue>();
			var objMessage = Substitute.For<IMessage>();
			objMessage.ID.Returns("MyID");
			objProvider.GetByteArray(Arg.Is(objMessage)).Returns(new byte[] { 1, 2, 3, 4 });

			Config.Initialize(new Config.ConfigParams() { MessageProvider = objProvider });

			//Act
			obj.Request(
				objClient,
				objMessage,
				Substitute.For<Action<IResponseMessage>>()
			);

			//Assert
			objClient.Received(1).Transmit(Arg.Is<byte[]>(b => b.SequenceEqual(new byte[] { 1, 2, 3, 4 })));
		}


		/// <summary>
		/// Tests what happens when SetResponse is called
		/// This means that SetResponse is called on the MessageRequest,
		/// the Action callback is invoked and the request is removed 
		/// from the Requests dictionary in RequestManager
		/// </summary>
		[Test]
		public void SetResponseTest() {
			//Arrange
			var obj = new RequestManager();

			var objClient = Substitute.For<IQueue.IQueue>();
			var objMessage = Substitute.For<IMessage>();
			var objResponse = Substitute.For<IResponseMessage>();
			objMessage.ID.Returns("MyID");
			objResponse.ID.Returns("MyID");

			int intActionCalled = 0;
			Action<IResponseMessage> objAction = delegate (IResponseMessage pMessage) {
				intActionCalled++;
			};

			//Act
			obj.Request(objClient, objMessage, objAction);
			obj.SetResponse(objResponse);

			//Assert
			Assert.IsTrue(intActionCalled == 1);
		}

	}
}