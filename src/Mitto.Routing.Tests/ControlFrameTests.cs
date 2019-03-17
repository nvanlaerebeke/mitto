using NUnit.Framework;
using NSubstitute;
using System.Linq;
using System;
using Mitto.IRouting;
using System.Text;
using System.Collections.Generic;

namespace Mitto.Routing.Tests {
	[TestFixture()]
	public class ControlFrameTests {

		/// <summary>
		/// Tests the creation of an IFrame by passing a byte array
		/// This means that the MessageType, name and data is returned successfully
		/// </summary>
		[Test]
		public void CreateWithByteArrayTest() {
			//Arrange
			byte[] arrID = Encoding.ASCII.GetBytes("MyID");
			byte[] arrName = Encoding.UTF32.GetBytes("MyCustomName");
			byte[] arrData = new byte[] { 1, 2, 3, 4, 5 };

			var lstFrame = new List<byte>();
			lstFrame.Add((byte)ControlFrameType.Response);
			lstFrame.Add((byte)arrID.Length);
			lstFrame.AddRange(arrID);
			lstFrame.Add((byte)arrName.Length);
			lstFrame.AddRange(arrName);
			lstFrame.AddRange(arrData);

			//Act
			var objFrame = new ControlFrame(lstFrame.ToArray());

			//Assert
			Assert.AreEqual(ControlFrameType.Response, objFrame.FrameType);
			Assert.AreEqual("MyID", objFrame.RequestID);
			Assert.AreEqual("MyCustomName", objFrame.MessageName);
			Assert.IsTrue(objFrame.Data.SequenceEqual(arrData));
			Assert.IsTrue(objFrame.GetBytes().SequenceEqual(lstFrame.ToArray()));
		}


		/// <summary>
		/// Tests the creation of a IFrame by passing each parameter separately 
		/// </summary>
		[Test]
		public void CreateWithParametersTest() {
			//Arrange
			byte[] arrID = Encoding.ASCII.GetBytes("MyID");
			byte[] arrName = Encoding.UTF32.GetBytes("MyCustomName");
			byte[] arrData = new byte[] { 1, 2, 3, 4, 5 };

			var lstFrame = new List<byte>();
			lstFrame.Add((byte)ControlFrameType.Response);
			lstFrame.Add((byte)arrID.Length);
			lstFrame.AddRange(arrID);
			lstFrame.Add((byte)arrName.Length);
			lstFrame.AddRange(arrName);
			lstFrame.AddRange(arrData);

			//Act
			var objFrame = new ControlFrame(ControlFrameType.Response, "MyCustomName", "MyID", arrData);

			//Assert
			Assert.AreEqual(ControlFrameType.Response, objFrame.FrameType);
			Assert.AreEqual("MyID", objFrame.RequestID);
			Assert.AreEqual("MyCustomName", objFrame.MessageName);
			Assert.IsTrue(objFrame.Data.SequenceEqual(arrData));
			Assert.IsTrue(objFrame.GetBytes().SequenceEqual(lstFrame.ToArray()));
		}
	}
}