﻿using Mitto.IMessaging;
using Mitto.IRouting;
using NUnit.Framework;

namespace Mitto.Messaging.Tests.Message.Request {
	[TestFixture]

	public class EchoTests {
		/// <summary>
		/// Test the Echo message creation
		/// This means that when creating the message the string is expected in the Message property
		/// </summary>
		[Test]
		public void CreateTest() {
			var objMessage = new Messaging.Request.EchoRequest("MyMessage");

			Assert.IsNotEmpty(objMessage.ID);
			Assert.AreEqual("EchoRequest", objMessage.Name);
			Assert.AreEqual(MessageType.Request, objMessage.Type);
			Assert.AreEqual("MyMessage", objMessage.Message);
		}
	}
}
