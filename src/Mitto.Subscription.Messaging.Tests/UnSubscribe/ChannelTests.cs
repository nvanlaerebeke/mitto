using Mitto.IMessaging;
using Mitto.IRouting;
using NUnit.Framework;

namespace Mitto.Subscription.Messaging.Tests.UnSubscribe {

    [TestFixture]
    public class ChannelTests {

        /// <summary>
        /// Tests the creation of the Channel UnSubscribe message
        /// </summary>
        [Test]
        public void CreateTest() {
            var obj = new Messaging.UnSubscribe.ChannelUnSubscribe("MyChannel");

            Assert.AreEqual("ChannelUnSubscribe", obj.Name);
            Assert.AreEqual("MyChannel", obj.ChannelName);
            Assert.AreEqual(MessageType.Request, obj.Type);
        }
    }
}