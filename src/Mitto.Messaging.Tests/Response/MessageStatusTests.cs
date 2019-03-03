using Mitto.IMessaging;
using NSubstitute;
using NUnit.Framework;

namespace Mitto.Messaging.Tests.Message.Response {
	[TestFixture]
	public class MessageStatusTests {
		/// <summary>
		/// Test the MessageStatus response message creation
		/// </summary>
		[Test]
		public void CreateTest() {
			//Arrange
			var objMessage = Substitute.For<Messaging.Request.MessageStatus>();
			objMessage.RequestID.Returns("MyID");

			//Act
			var obj = new Messaging.Response.MessageStatus(objMessage, MessageStatusType.Busy);

			//Assert
			Assert.AreEqual("MessageStatus", obj.Name);
			Assert.AreEqual(objMessage, obj.Request);
			Assert.AreEqual(ResponseCode.Success, obj.Status);
			Assert.AreEqual(MessageType.Response, obj.Type);
			Assert.AreEqual(ResponseCode.Success, obj.Status);
			Assert.AreEqual(MessageStatusType.Busy, obj.RequestStatus);
		}
	}
}