using Mitto.IMessaging;
using NSubstitute;
using NUnit.Framework;

namespace Mitto.Messaging.Tests.Action.Control {
	[TestFixture]
	public class MessageStatusTests {
		/// <summary>
		/// Tests the MessageStatus action for the requests message messagestatus
		/// This means an Response.MessageStatus message is expected with as
		/// message the status response a MessageStatusType
		/// </summary>
		[Test]
		public void CreateTest() {
			//Arrange
			var objProcessor = Substitute.For<IMessageProcessor>();
			var objClient = Substitute.For<IClient>();
			var objMessage = Substitute.For<Messaging.Control.MessageStatusRequest>("RequestIDFromRequested");
			objMessage.ID.Returns("MyRequestID");
			objProcessor.GetStatus(Arg.Is("RequestIDFromRequested")).Returns(MessageStatusType.Queued);

			Config.Initialize(new Config.ConfigParams() {
				MessageProcessor = objProcessor
			});

			//Act
			var objAction = new Messaging.Action.Control.MessageStatusRequestAction(objClient, objMessage);
			var objResponse = objAction.Start() as Response.MessageStatusResponse;

			//Assert
			Assert.IsNotNull(objAction);
			Assert.IsNotNull(objResponse);
			Assert.AreEqual(MessageStatusType.Queued, objResponse.RequestStatus);
			Assert.AreEqual("MyRequestID", objResponse.ID);
		}
	}
}