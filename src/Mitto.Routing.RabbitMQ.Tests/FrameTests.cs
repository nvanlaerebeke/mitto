using System.Collections;
using System.Collections.Generic;
using Mitto.IMessaging;
using NUnit.Framework;
using System.Linq;

namespace Mitto.Routing.RabbitMQ.Tests {
	[TestFixture]
	public class FrameTests {
		[Test]
		public void CreateFrameByteArrayTest() {
			//Arrange
			var objMessageFrame = new IMessaging.Frame(MessageType.Request, "MyID", "MyName", new byte[] { 1, 2, 3, 4, 5 });
			var arrQueueID = System.Text.Encoding.ASCII.GetBytes("MyQueueID");
			var arrConnectionID = System.Text.Encoding.ASCII.GetBytes("MyConnectionID");

			var lstFrame = new List<byte>();
			lstFrame.Add((byte)MessageType.Request);
			lstFrame.Add((byte)arrQueueID.Length);
			lstFrame.AddRange(arrQueueID);
			lstFrame.Add((byte)arrConnectionID.Length);
			lstFrame.AddRange(arrConnectionID);
			lstFrame.AddRange(objMessageFrame.GetByteArray());

			//Act
			var objFrame = new Frame(lstFrame.ToArray());

			//Assert
			Assert.AreEqual(MessageType.Request, objFrame.MessageType);
			Assert.AreEqual("MyQueueID", objFrame.QueueID);
			Assert.AreEqual("MyConnectionID", objFrame.ConnectionID);
			Assert.AreEqual("MyID", objFrame.MessageID);
			Assert.IsTrue(objFrame.Data.SequenceEqual(objMessageFrame.GetByteArray()));
		}

		[Test]
		public void CreateFrameWithParamsTest() {
			//Arrange
			var objMessageFrame = new IMessaging.Frame(MessageType.Request, "MyID", "MyName", new byte[] { 1, 2, 3, 4, 5 });

			//Act
			var objFrame = new Frame("MyQueueID", "MyConnectionID", objMessageFrame.GetByteArray());

			//Assert
			Assert.AreEqual(MessageType.Request, objFrame.MessageType);
			Assert.AreEqual("MyQueueID", objFrame.QueueID);
			Assert.AreEqual("MyConnectionID", objFrame.ConnectionID);
			Assert.AreEqual("MyID", objFrame.MessageID);
			Assert.IsTrue(objFrame.Data.SequenceEqual(objMessageFrame.GetByteArray()));
		}
	}
}
