﻿using Mitto.IMessaging;
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
			var objMessage = Substitute.For<Messaging.Request.Echo>();
			objMessage.ID.Returns("MyID");
			objMessage.Message = "MyCustomMessage";

			var obj = new Messaging.Response.Echo(objMessage, ResponseCode.Success);

			Assert.AreEqual("MyID", obj.ID);
			Assert.AreEqual("Echo", obj.Name);
			Assert.AreEqual(obj.Request, objMessage);
			Assert.AreEqual(ResponseCode.Success, obj.Status);
			Assert.AreEqual(MessageType.Response, obj.Type);
			Assert.AreEqual("MyCustomMessage", obj.Message);
		}
	}
}