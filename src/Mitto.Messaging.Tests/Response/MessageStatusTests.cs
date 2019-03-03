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
			var objMessage = Substitute.For<Messaging.Request.MessageStatusRequest>();
			objMessage.RequestID = "MyID";

			//Act
			var obj = new Messaging.Response.MessageStatusResponse(objMessage, MessageStatusType.Busy);

			//Assert
			Assert.AreEqual("MessageStatusResponse", obj.Name);
			Assert.AreEqual(objMessage, obj.Request);
			Assert.AreEqual(ResponseState.Success, obj.Status.State);
			Assert.AreEqual(MessageType.Response, obj.Type);
			Assert.AreEqual(MessageStatusType.Busy, obj.RequestStatus);
		}
	}
}