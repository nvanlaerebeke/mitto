using System.Collections;
using System.Collections.Generic;
using Mitto.IRouting;
using NUnit.Framework;
using System.Linq;
using Mitto.IMessaging;

namespace Mitto.Routing.RabbitMQ.Tests {
	[TestFixture]
	public class FrameTests {
		[Test]
		public void CreateFrameByteArrayTest() {
			//Arrange
			var objMessageFrame = new Frame(MessageType.Request, "MyID", "MyName", new byte[] { 1, 2, 3, 4, 5 });
			var arrConnectionID = System.Text.Encoding.ASCII.GetBytes("MyConnectionID");

			var lstFrame = new List<byte>();
			lstFrame.Add((byte)RoutingFrameType.Messaging);
			lstFrame.Add((byte)arrConnectionID.Length);
			lstFrame.AddRange(arrConnectionID);
			lstFrame.AddRange(objMessageFrame.GetByteArray());

			//Act
			var objFrame = new RoutingFrame(lstFrame.ToArray());

			//Assert
			Assert.AreEqual(RoutingFrameType.Messaging, objFrame.FrameType);
			Assert.AreEqual("MyConnectionID", objFrame.ConnectionID);
			Assert.IsTrue(objFrame.Data.SequenceEqual(objMessageFrame.GetByteArray()));
		}

		[Test]
		public void CreateFrameWithParamsTest() {
			//Arrange
			var objMessageFrame = new Frame(MessageType.Request, "MyID", "MyName", new byte[] { 1, 2, 3, 4, 5 });

			//Act
			var objFrame = new RoutingFrame(RoutingFrameType.Messaging, "MyConnectionID", objMessageFrame.GetByteArray());

			//Assert
			Assert.AreEqual(RoutingFrameType.Messaging, objFrame.FrameType);
			Assert.AreEqual("MyConnectionID", objFrame.ConnectionID);
			Assert.IsTrue(objFrame.Data.SequenceEqual(objMessageFrame.GetByteArray()));
		}
	}
}
