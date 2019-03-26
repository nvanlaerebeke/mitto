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
			var arrRequestID = System.Text.Encoding.ASCII.GetBytes("MyID");
			var arrSourceID = System.Text.Encoding.ASCII.GetBytes("MySourceID");
			var arrDestinationID = System.Text.Encoding.ASCII.GetBytes("MyDestinationID");

			var lstFrame = new List<byte>();
			lstFrame.Add((byte)RoutingFrameType.Messaging);
			lstFrame.Add((byte)arrRequestID.Length);
			lstFrame.AddRange(arrRequestID);
			lstFrame.Add((byte)arrSourceID.Length);
			lstFrame.AddRange(arrSourceID);
			lstFrame.Add((byte)arrDestinationID.Length);
			lstFrame.AddRange(arrDestinationID);
			lstFrame.AddRange(objMessageFrame.GetByteArray());

			//Act
			var objFrame = new RoutingFrame(lstFrame.ToArray());

			//Assert
			Assert.AreEqual(RoutingFrameType.Messaging, objFrame.FrameType);
			Assert.AreEqual("MySourceID", objFrame.SourceID);
			Assert.AreEqual("MyDestinationID", objFrame.DestinationID);
			Assert.IsTrue(objFrame.Data.SequenceEqual(objMessageFrame.GetByteArray()));
		}

		[Test]
		public void CreateFrameWithParamsTest() {
			//Arrange
			var objMessageFrame = new Frame(MessageType.Request, "MyID", "MyName", new byte[] { 1, 2, 3, 4, 5 });

			//Act
			var objFrame = new RoutingFrame(RoutingFrameType.Messaging, "MyID", "MySourceID", "MyDestinationID", objMessageFrame.GetByteArray());

			//Assert
			Assert.AreEqual(RoutingFrameType.Messaging, objFrame.FrameType);
			Assert.AreEqual("MyID", objFrame.RequestID);
			Assert.AreEqual("MySourceID", objFrame.SourceID);
			Assert.AreEqual("MyDestinationID", objFrame.DestinationID);
			Assert.IsTrue(objFrame.Data.SequenceEqual(objMessageFrame.GetByteArray()));
		}
	}
}
