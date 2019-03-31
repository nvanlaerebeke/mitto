using NUnit.Framework;
using NSubstitute;
using System.Linq;
using System;
using Mitto.IRouting;
using Mitto.Routing.RabbitMQ;
using System.Collections.Generic;

namespace Mitto.Routing.PassThrough.Tests {
	/*[TestFixture()]
	public class RabbitMQFrameTests {

		[Test]
		public void CreateFrameByteArrayTest() {
			//Arrange
			var arrSourceID = System.Text.Encoding.ASCII.GetBytes("MySourceID");
			var arrDestinationID = System.Text.Encoding.ASCII.GetBytes("MyDestinationID");

			var lstFrame = new List<byte>();
			lstFrame.Add((byte)RoutingFrameType.Messaging);
			lstFrame.Add((byte)arrSourceID.Length);
			lstFrame.AddRange(arrSourceID);
			lstFrame.Add((byte)arrDestinationID.Length);
			lstFrame.AddRange(arrDestinationID);
			lstFrame.AddRange(new byte[] { 1, 2, 3, 4, 5 });

			//Act
			var objFrame = new RabbitMQFrame(lstFrame.ToArray());

			//Assert
			Assert.AreEqual(RoutingFrameType.Messaging, objFrame.FrameType);
			Assert.AreEqual("MySourceID", objFrame.SourceID);
			Assert.AreEqual("MyDestinationID", objFrame.DestinationID);
			Assert.IsTrue(objFrame.Data.SequenceEqual(new byte[] { 1, 2, 3, 4, 5 }));
		}

		[Test]
		public void CreateFrameWithParamsTest() {
			//Arrange
			//Act
			var objFrame = new RabbitMQFrame(RoutingFrameType.Messaging, "MySourceID", "MyDestinationID", new byte[] { 1, 2, 3, 4, 5 });

			//Assert
			Assert.AreEqual(RoutingFrameType.Messaging, objFrame.FrameType);
			Assert.AreEqual("MySourceID", objFrame.SourceID);
			Assert.AreEqual("MyDestinationID", objFrame.DestinationID);
			Assert.IsTrue(objFrame.Data.SequenceEqual(new byte[] { 1, 2, 3, 4, 5 }));
		}
	}*/
}