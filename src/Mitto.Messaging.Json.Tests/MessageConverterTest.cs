using NUnit.Framework;
using NSubstitute;
using Mitto.IMessaging;

namespace Mitto.Messaging.Json.Tests {
	[TestFixture]
	public class MessageConverterTest {
		/// <summary>
		/// Tests if a correct response message is returned
		/// This means that when an IMessage is passed, an IResponseMessage for that
		/// type with the given response code is expected
		/// </summary>
		[Test]
		public void GetResponseMessageTest() {
			//Arrange
			var objRequest = Substitute.For<IMessage>();
			var objProvider = Substitute.For<IMessageProvider>();
			objRequest.Name.Returns("TestMessageResponse");
			objProvider.GetResponseType("TestMessageResponse").Returns(typeof(Libs.TestMessageResponse));
			Mitto.Initialize(new Mitto.Config() {
				MessageProvider = objProvider
			});

			//Act
			var objResponse = new MessageConverter().GetResponseMessage(objRequest, ResponseCode.Cancelled);

			//Assert
			Assert.AreEqual(objResponse.Name, "TestMessageResponse");
			Assert.IsTrue(objResponse.Request.Equals(objRequest));
			Assert.AreEqual(objResponse.Type, MessageType.Response);
			Assert.AreEqual(objResponse.Status, ResponseCode.Cancelled);
			Assert.AreEqual(objResponse.GetCode(), 0x66);
		}


		/// <summary>
		/// Tests if a correct IMessage/byte[] is returned based on the provided TestMessage
		/// This means that when a byte array is passed the resulting IMessage is returned
		/// GetByteArray is used to get the initial byte array to pass to GetMessage, so both
		/// methods are being tested in this test case
		/// </summary>
		[Test]
		public void MessageConversionTest() {
			//Arrange
			var arrMessage = new MessageConverter().GetByteArray(new Libs.TestMessage());
			var objProvider = Substitute.For<IMessageProvider>();
			objProvider.GetType(MessageType.Request, 0x66).Returns(typeof(Libs.TestMessage));

			Mitto.Initialize(new Mitto.Config() {
				MessageProvider = objProvider
			});

			//Act
			var objConverter = new MessageConverter();
			IMessage objMessage = objConverter.GetMessage(arrMessage);

			//Assert
			Assert.AreEqual(objMessage.ID, "MyID");
			Assert.AreEqual(objMessage.Name, "TestMessage");
			Assert.AreEqual(objMessage.Type, MessageType.Request);
		}

	}

}
