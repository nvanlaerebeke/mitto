using NUnit.Framework;
using NSubstitute;
using Mitto.IMessaging;
using System.Linq;
using Mitto.IRouting;

namespace Mitto.Messaging.Json.Tests {
	[TestFixture]
	public class MessageConverterTest {


		/// <summary>
		/// Tests if a correct IMessage/byte[] is returned based on the provided TestMessage
		/// This means that when a byte array is passed with it's type the resulting IMessage is returned
		/// GetByteArray is used to get the initial byte array to pass to GetMessage, so both
		/// methods are being tested in this test case
		/// </summary>
		[Test]
		public void GetMessageTest() {
			//Arrange
			var objMessage = new Libs.TestMessage();
			var arrMessage = new MessageConverter().GetByteArray(objMessage);

			//Act
			var objConverter = new MessageConverter();
			IMessage objConvertedMessage = objConverter.GetMessage(typeof(Libs.TestMessage), arrMessage);
			byte[] arrMessageData = objConverter.GetByteArray(objMessage);
			

			//Assert
			Assert.AreEqual(objConvertedMessage.ID, "MyID");
			Assert.AreEqual(objConvertedMessage.Name, "TestMessage");
			Assert.AreEqual(objConvertedMessage.Type, MessageType.Request);

			Assert.IsTrue(arrMessageData.SequenceEqual(arrMessage));
		}
	}
}