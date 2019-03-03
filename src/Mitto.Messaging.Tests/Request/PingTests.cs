using Mitto.IMessaging;
using NUnit.Framework;

namespace Mitto.Messaging.Tests.Message.Request {
	[TestFixture]
	public class PingTests {
		/// <summary>
		/// Test the Ping message creation
		/// </summary>
		[Test]
		public void CreateTest() {
			var objMessage = new Messaging.Request.PingRequest();

			Assert.IsNotEmpty(objMessage.ID);
			Assert.AreEqual("PingRequest", objMessage.Name);
			Assert.AreEqual(MessageType.Request, objMessage.Type);
		}
	}
}
