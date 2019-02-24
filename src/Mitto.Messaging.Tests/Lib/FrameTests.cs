﻿using Mitto.IMessaging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mitto.Messaging.Tests {
	[TestFixture]
	public class FrameTests {
		/// <summary>
		/// Tests the creation of an IFrame by passing a byte array
		/// This means that the MessageType, name and data is returned successfully
		/// </summary>
		[Test]
		public void CreateWithByteArrayTest() {
			//Arrange
			byte bytMessageType = (byte)MessageType.Request;
			byte[] arrName = Encoding.UTF32.GetBytes("MyCustomName");
			byte[] arrData = new byte[] { 1, 2, 3, 4, 5 };
			var lstFrame = new List<byte>();
			lstFrame.Add(bytMessageType);
			lstFrame.Add((byte)arrName.Length);
			lstFrame.AddRange(arrName);
			lstFrame.AddRange(arrData);

			//Act
			var objFrame = new Frame(lstFrame.ToArray());

			//Assert
			Assert.AreEqual(objFrame.Name, "MyCustomName");
			Assert.AreEqual(objFrame.Type, MessageType.Request);
			Assert.IsTrue(objFrame.Data.SequenceEqual(arrData));
		}


		/// <summary>
		/// Tests the creation of a IFrame by passing each parameter separately 
		/// </summary>
		[Test]
		public void CreateWithParametersTest() {
			//Arrange
			byte bytMessageType = (byte)MessageType.Request;
			byte[] arrName = Encoding.UTF32.GetBytes("MyCustomName");
			byte[] arrData = new byte[] { 1, 2, 3, 4, 5 };

			var lstFrame = new List<byte>();
			lstFrame.Add(bytMessageType);
			lstFrame.Add((byte)arrName.Length);
			lstFrame.AddRange(arrName);
			lstFrame.AddRange(arrData);

			//Act
			var objFrame = new Frame(MessageType.Request, "MyCustomName", arrData);

			//Assert
			Assert.AreEqual(objFrame.Type, MessageType.Request);
			Assert.AreEqual(objFrame.Name, "MyCustomName");
			Assert.IsTrue(lstFrame.ToArray().SequenceEqual(objFrame.GetByteArray()));
		}
	}
}
