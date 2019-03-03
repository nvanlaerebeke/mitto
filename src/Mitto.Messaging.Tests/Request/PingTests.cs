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
			var objMessage = new Messaging.Request.Ping();

			Assert.IsNotEmpty(objMessage.ID);
			Assert.AreEqual("Ping", objMessage.Name);
			Assert.AreEqual(MessageType.Request, objMessage.Type);
		}
	}
}
