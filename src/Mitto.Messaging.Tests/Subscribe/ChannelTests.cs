using Mitto.IMessaging;
using NUnit.Framework;

namespace Mitto.Messaging.Tests.Subscribe {
	[TestFixture]
	public class ChannelTests {
		/// <summary>
		/// Tests the creation of the Channel subscribe message
		/// </summary>
		[Test]
		public void CreateTest() {
			var obj = new Messaging.Subscribe.Channel("MyChannel");
			Assert.AreEqual("MyChannel", obj.ChannelName);
			Assert.AreEqual(MessageType.Sub, obj.Type);
		}
	}
}
