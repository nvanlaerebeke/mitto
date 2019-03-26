using Mitto.IMessaging;
using Mitto.IRouting;
using NSubstitute;
using NUnit.Framework;

namespace Mitto.Messaging.Tests.Message.Response {
	[TestFixture]
	public class EchoTests {
		/// <summary>
		/// Test the Echo response message creation
		/// </summary>
		[Test]
		public void CreateTest() {
			var objMessage = Substitute.For<Messaging.Request.EchoRequest>();
			objMessage.ID.Returns("MyID");
			objMessage.Message = "MyCustomMessage";

			var obj = new Messaging.Response.EchoResponse(objMessage);

			Assert.AreEqual("MyID", obj.ID);
			Assert.AreEqual("EchoResponse", obj.Name);
			Assert.AreEqual(ResponseState.Success, obj.Status.State);
			Assert.AreEqual(MessageType.Response, obj.Type);
			Assert.AreEqual("MyCustomMessage", obj.Message);
		}
	}
}