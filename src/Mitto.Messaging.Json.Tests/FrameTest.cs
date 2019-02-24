using Mitto.IMessaging;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Mitto.Messaging.Json.Tests {
	[TestFixture]
	public class FrameTest {

		/// <summary>
		/// Tests the creation of an IFrame by passing a byte array
		/// This means that the Format and data is returned successfully
		/// </summary>
		[Test]
		public void CreateWithByteArrayTest() {
			//Arrange
			byte bytMessageType = (byte)MessageFormat.Json;
			byte[] arrData = new byte[] { 1, 2, 3, 4, 5 };

			var lstFrame = new List<byte>();
			lstFrame.Add(bytMessageType);
			lstFrame.AddRange(arrData);

			//Act
			var objFrame = new Frame(lstFrame.ToArray());

			//Assert
			Assert.AreEqual(objFrame.Format, MessageFormat.Json);
			Assert.IsTrue(objFrame.Data.SequenceEqual(arrData));
		}


		/// <summary>
		/// Tests the creation of a IFrame by passing each parameter separately 
		/// </summary>
		[Test]
		public void CreateWithParametersTest() {
			//Arrange
			byte bytMessageType = (byte)MessageFormat.Json;
			byte[] arrData = new byte[] { 1, 2, 3, 4, 5 };

			var lstFrame = new List<byte>();
			lstFrame.Add(bytMessageType);
			lstFrame.AddRange(arrData);

			//Act
			var objFrame = new Frame(MessageFormat.Json, arrData);

			//Assert
			Assert.AreEqual(MessageFormat.Json, objFrame.Format);
			Assert.IsTrue(arrData.SequenceEqual(objFrame.Data));
		}
	}
}