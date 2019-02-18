using Mitto.IMessaging;
using NUnit.Framework;
using System.Linq;

namespace Mitto.Messaging.Json.Tests {
	[TestFixture]
	public class FrameTest {

		/// <summary>
		/// Tests the creation of an IFrame by passing a byte array
		/// This means that the Format/MessageType/MessageCode and data is returned successfully
		/// </summary>
		[Test]
		public void CreateWithByteArrayTest() {
			var objFrame = new Frame(new byte[] { 1, 2, 53, 1, 2, 3, 4 });

			//Assert
			Assert.AreEqual(MessageFormat.Json, objFrame.Format);
			Assert.AreEqual(MessageType.Request, objFrame.Type);
			Assert.AreEqual(53, objFrame.Code);
			Assert.IsTrue((new byte[] { 1, 2, 3, 4 }).SequenceEqual(objFrame.Data));
		}


		/// <summary>
		/// Tests the creation of a IFrame by passing each parameter separately 
		/// </summary>
		[Test]
		public void CreateWithParametersTest() {
			var objFrame = new Frame(MessageFormat.Json, MessageType.Request, 53, new byte[] { 1, 2, 3, 4 });

			//Assert
			Assert.AreEqual(MessageFormat.Json, objFrame.Format);
			Assert.AreEqual(MessageType.Request, objFrame.Type);
			Assert.AreEqual(53, objFrame.Code);
			Assert.IsTrue((new byte[] { 1, 2, 3, 4 }).SequenceEqual(objFrame.Data));
		}
	}
}
