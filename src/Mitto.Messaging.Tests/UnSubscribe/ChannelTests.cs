using Mitto.IMessaging;
using NUnit.Framework;

namespace Mitto.Messaging.Tests.UnSubscribe {
	[TestFixture]
	public class ChannelTests {
		/// <summary>
		/// Tests the creation of the Channel UnSubscribe message
		/// </summary>
		[Test]
		public void CreateTest() {
			var obj = new Messaging.UnSubscribe.ChannelUnSubscribe("MyChannel");
			Assert.AreEqual("MyChannel", obj.ChannelName);
			Assert.AreEqual(MessageType.UnSub, obj.Type);
		}
	}
}
