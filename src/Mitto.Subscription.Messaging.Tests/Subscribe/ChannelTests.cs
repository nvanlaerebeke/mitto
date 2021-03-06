﻿using Mitto.IMessaging;
using Mitto.IRouting;
using NUnit.Framework;

namespace Mitto.Subscription.Messaging.Tests.Subscribe {

    [TestFixture]
    public class ChannelTests {

        /// <summary>
        /// Tests the creation of the Channel subscribe message
        /// </summary>
        [Test]
        public void CreateTest() {
            var obj = new Messaging.Subscribe.ChannelSubscribe("MyChannel");

            Assert.AreEqual("ChannelSubscribe", obj.Name);
            Assert.AreEqual("MyChannel", obj.ChannelName);
            Assert.AreEqual(MessageType.Request, obj.Type);
        }
    }
}